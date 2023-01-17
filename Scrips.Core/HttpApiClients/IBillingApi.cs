using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using Scrips.Core.Models.Billing;

namespace Scrips.Core.HttpApiClients;

public interface IBillingApi
{
    [Get("/api/Billing/ProviderBillingProfiles")]
    Task<ProviderBillingProfileDto> ProviderBillingProfiles([Query] Guid providerId,
        [Query] Guid appointmentProfileId, [Header("Authorization")] string token);

    [Post("/api/Billing/Agreements/CalculatePrice")]
    Task<FeeSummaryResponse> AgreementsCalculatePrice([Header("OrganizationId")] Guid organizationId,
        [Body] CalculatePriceRequest body, [Header("Authorization")] string token);

    [Post("/api/Billing/Agreements/PreCalculatePrice")]
    Task<FeeSummaryResponse> AgreementsPreCalculatePrice([Header("OrganizationId")] Guid organizationId,
        [Body] PreCalculatePriceRequest body, [Header("Authorization")] string token);

    [Get("/api/Billing/Invoice/{invoiceId}")]
    Task<InvoiceDto> InvoiceGetById(Guid invoiceId, [Header("Authorization")] string token);

    [Get("/api/Billing/Payment/AvailableBalance")]
    Task<AvailableBalanceResponse> PaymentAvailableBalance([Query] Guid patientId,
        [Header("Authorization")] string token);

    [Get("/api/Billing/Payment/PaymentReceipts")]
    Task<List<PaymentReceiptResponse>> PaymentPaymentReceipts([Query] Guid invoiceId,
        [Header("OrganizationId")] Guid organizationId, [Header("Authorization")] string token);
        
    [Get("/api/Billing/Payment/PaymentReceipts2")]
    Task<List<PaymentReceiptResponse>> PaymentPaymentReceipts2([Query] Guid invoiceId,
        [Header("OrganizationId")] Guid organizationId, [Header("Authorization")] string token);
}