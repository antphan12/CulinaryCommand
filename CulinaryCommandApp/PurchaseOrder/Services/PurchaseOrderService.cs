using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CulinaryCommand.Data;
using CulinaryCommand.PurchaseOrder.DTOs;
using POEntities = CulinaryCommand.PurchaseOrder.Entities;
using CulinaryCommand.Components;
using System.Threading.Tasks.Dataflow;

namespace CulinaryCommand.PurchaseOrder.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly AppDbContext _db;

        public PurchaseOrderService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<PurchaseOrderDTO> CreateDraftAsync(CreatePurchaseOrderDTO request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            
            if (request.Lines == null || !request.Lines.Any())
                throw new ArgumentException("At least one line is required.", nameof(request));

            var locationExists = await _db.Locations
                .AnyAsync(location => location.Id == request.LocationId, cancellationToken);

            if (!locationExists)
                throw new InvalidOperationException("Location not found.");

            var ingredientIds = request.Lines.Select(line => line.IngredientId).Distinct().ToList();
            var unitIds = request.Lines.Select(line => line.UnitId).Distinct().ToList();

            var existingIngredientIds = await _db.Ingredients
                .Where(ingredient => ingredientIds.Contains(ingredient.Id))
                .Select(ingredient => ingredient.Id)
                .ToListAsync(cancellationToken);
            
            var existingUnitIds = await _db.Units
                .Where(unit => unitIds.Contains(unit.Id))
                .Select(unit => unit.Id)
                .ToListAsync(cancellationToken);

            var missingIngredients = ingredientIds.Except(existingIngredientIds).ToList();
            if (missingIngredients.Any())
                throw new InvalidOperationException ("Some ingredients are invalid: " + string.Join(", ", missingIngredients));

            var missingUnits = unitIds.Except(existingUnitIds).ToList();
            if (missingUnits.Any())
                throw new InvalidOperationException("Some units are invalid: " + string.Join(", ", missingUnits));

            var purchaseOrder = new POEntities.PurchaseOrder
            {
                OrderNumber = GenerateOrderNumber(),
                LocationId = request.LocationId,
                VendorName = request.VendorName,
                VendorContact = request.VendorContact,
                ExpectedDeliveryDate = request.ExpectedDeliveryDate,
                Notes = request.Notes,
                Status = POEntities.PurchaseOrderStatus.Draft,
                IsLocationLocked = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            foreach (var lineDTO in request.Lines)
            {
                purchaseOrder.Lines.Add(new POEntities.PurchaseOrderLine
                {
                    IngredientId = lineDTO.IngredientId,
                    UnitId = lineDTO.UnitId,
                    QuantityOrdered = lineDTO.QuantityOrdered,
                    QuantityReceived = 0m,
                    UnitPrice = lineDTO.UnitPrice
                });
            }

            _db.PurchaseOrders.Add(purchaseOrder);
            await _db.SaveChangesAsync(cancellationToken);

            // map back to DTO
            return new PurchaseOrderDTO
            {
                Id = purchaseOrder.Id,
                OrderNumber = purchaseOrder.OrderNumber,
                LocationId = purchaseOrder.LocationId,
                VendorName = purchaseOrder.VendorName,
                VendorContact = purchaseOrder.VendorContact,
                ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate,
                Status = purchaseOrder.Status,
                Notes = purchaseOrder.Notes,
                Lines = purchaseOrder.Lines.Select(line => new PurchaseOrderLineDTO
                {
                    Id = line.Id,
                    IngredientId = line.IngredientId,
                    UnitId = line.UnitId,
                    QuantityOrdered = line.QuantityOrdered,
                    QuantityReceived = line.QuantityReceived,
                    UnitPrice = line.UnitPrice
                }).ToList()
            };
        }

        // helper function to generate an order number
        private static string GenerateOrderNumber()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"PO-{timestamp}-{random}";
        }
    }
}