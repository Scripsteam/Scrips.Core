using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using Scrips.Core.Models.Scheduling;

namespace Scrips.Core.HttpApiClients;

public interface ISchedulingApi
{
    [Post("/api/Appointment/Slots")]
    Task<List<SlotResponse>> AppointmentSlots([Body] SlotsRequest slotsRequest,
        [Header("Authorization")] string auth, [Header("OrganizationID")] Guid organizationId);
        
    [Post("/api/Appointment/Slots2")]
    Task<List<SlotResponse>> AppointmentSlots2([Body] SlotsRequest slotsRequest,
        [Header("Authorization")] string auth, [Header("OrganizationID")] Guid organizationId);


    [Get("/api/Appointment/PatientFlag")]
    Task<List<FlagResponse>> AppointmentPatientFlag([Query] Guid patientId, [Header("Authorization")] string auth);

    [Get("/api/Appointment/GetById/{id}")]
    Task<AppointmentResponse> AppointmentGetById(Guid id, [Header("Authorization")] string auth);
}