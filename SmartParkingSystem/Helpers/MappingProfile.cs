using AutoMapper;
using SmartParking.Core.Entities;
using SmartParkingSystem.DTOs;
using SmartParkingSystem.DTOs.Requests;
using SmartParkingSystem.DTOs.Responses;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User
        CreateMap<User, UserResponseDTO>();
        CreateMap<CreateUserRequestDTO, User>();

        // Reservation
        CreateMap<Reservation, ReservationResponseDTO>();
        CreateMap<CreateReservationRequestDTO, Reservation>();

        // Wallet
        CreateMap<Wallet, WalletResponseDTO>();
        CreateMap<CreateWalletRequestDTO, Wallet>();

        // Payment
        CreateMap<Payment, PaymentResponseDTO>();
        CreateMap<CreatePaymentRequestDTO, Payment>();

        // Sensor
        CreateMap<Sensor, SensorResponseDTO>();
        CreateMap<CreateSensorRequestDTO, Sensor>();

        // AdminReport
        CreateMap<AdminReport, AdminReportResponseDTO>();
        CreateMap<CreateAdminReportRequestDTO, AdminReport>();

        // ChatbotLog
        CreateMap<ChatbotLog, ChatbotLogResponseDTO>();
        CreateMap<CreateChatbotLogRequestDTO, ChatbotLog>();

        // CheckInOutLog
        CreateMap<CheckInOutLog, CheckInOutLogResponseDTO>();
        CreateMap<CreateCheckInOutLogRequestDTO, CheckInOutLog>();

        // ParkingSlot
        CreateMap<ParkingSlot, ParkingSlotResponseDTO>();
        CreateMap<CreateParkingSlotRequestDTO, ParkingSlot>();

    }
}
