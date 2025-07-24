using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.OrderManagement.Admin.Queries.GetOrderDetailByAdmin;
using game_x.application.Features.OrderManagement.Client.Queries.GetOrderDetailByClient;
using game_x.application.Features.OrderManagement.Dtos;
using game_x.application.Features.OrderManagement.Staff.Commands.TraceV2.Buy;
using game_x.application.Features.OrderManagement.Staff.Commands.TraceV2.Sell;
using game_x.application.Features.OrderManagement.Staff.Commands.TraceV2.EstimateQuote;
using game_x.application.Features.OrderManagement.Staff.Commands.Trade.Buy;
using game_x.application.Features.OrderManagement.Staff.Commands.Trade.Sell;
using game_x.application.Features.OrderManagement.Staff.Queries.GetOrderDetailByStaff;
using game_x.share.ExternalApi.Uxm.Dtos;
using MapsterMapper;
using BankAccountEntity = game_x.domain.Entities.BankAccount;
using OrderEntity = game_x.domain.Entities.Order;

namespace game_x.application.Mappers.Order;

public sealed class OrderMapper(IMapper mapper)
{
    public CreateOrderBuyRequestData ToCreateOrderBuyRequestData(
        CreateBuyOrderCommand command,
        string publicId,
        string merchantNumber)
    {
        var result = mapper.Map<CreateOrderBuyRequestData>(command);
        return result with { MerchantOrderId = publicId, MerchantNumber = merchantNumber };
    }

    public CreateBuyOrderV2ReqData ToCreateOrderV2ReqData(CreateBuyOrderV2Command command)
    {
        var result = mapper.Map<CreateBuyOrderV2ReqData>(command);
        return result;
    }

    public CreateOrderSellRequestData ToCreateOrderSellRequestData(
        CreateSellOrderCommand command,
        string publicId,
        string merchantNumber,
        BankAccountEntity bankAccount)
    {
        var result = mapper.Map<CreateOrderSellRequestData>(command);
        return result with
        {
            MerchantOrderId = publicId,
            MerchantNumber = merchantNumber,
            PayeeBankName = bankAccount.BankName,
            PayeeBranchCode = bankAccount.BranchName,
            PayeeAccountNumber = bankAccount.BankAccountNumber,
            PayeeName = bankAccount.BankAccountName,
        };
    }
    
    public CreateSellOrderV2Request ToCreateSellOrderV2RequestData(
        CreateSellOrderV2Command command,
        string publicId,
        string merchantNumber)
    {
        var result = mapper.Map<CreateSellOrderV2Request>(command);
        return result with {
            MerchantOrderId = publicId,
            MerchantNumber = merchantNumber,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
    }

    public CreateOrderResponseDto ToCreateOrderDto(CreateOrderBuyResponseData data, bool isValid)
    {
        var dto = mapper.Map<CreateOrderResponseDto>(data);
        dto.IsValid = isValid;
        return dto;
    }
    
    public CreateOrderResponseDto ToCreateOrderDto(CreateOrderSellResponseData data, bool isValid)
    {
        var dto = mapper.Map<CreateOrderResponseDto>(data);
        dto.IsValid = isValid;
        return dto;
    }
    
    public CreateOrderV2ResponseDto ToCreateOrderV2Dto(CreateBuyOrderV2ResponseData data, bool isValid)
    {
        var dto = mapper.Map<CreateOrderV2ResponseDto>(data);
        dto.IsValid = isValid;
        return dto;
    }
    
    public CreateOrderV2ResponseDto ToCreateOrderV2Dto(CreateSellOrderV2Response data, bool isValid)
    {
        var dto = mapper.Map<CreateOrderV2ResponseDto>(data);
        dto.IsValid = isValid;
        return dto;
    }

    public PaginationResult<T> ToSearchResult<T>(
        PaginationResult<OrderEntity> data,
        Func<OrderEntity, T> mappingFunc)
        where T : class
    {
        var result = new PaginationResult<T>(
            items: data.Items.Select(item => mappingFunc(item)).ToList(),
            totalItems: data.TotalItems,
            totalPages: data.TotalPages,
            pageIndex: data.PageNumber,
            pageSize: data.PageSize);
        return result;
    }

    public GetUxmOrderDetailInfoRequest ToGetOrderDetailInfoReqData(
        GetOrderDetailByStaffQuery query,
        string merchantNumber,
        string tradeNo,
        long timestamp)
    {
        var result = mapper.Map<GetUxmOrderDetailInfoRequest>(query);
        return result with {
            MerchantNumber = merchantNumber,
            TradeNo = tradeNo,
            Timestamp = timestamp
        };
    }
    
    public GetUxmOrderDetailInfoRequest ToGetOrderDetailInfoReqData(
        GetOrderDetailByAdminQuery query,
        string merchantNumber,
        string tradeNo,
        long timestamp)
    {
        var result = mapper.Map<GetUxmOrderDetailInfoRequest>(query);
        return result with {
            MerchantNumber = merchantNumber,
            TradeNo = tradeNo,
            Timestamp = timestamp
        };
    }
    
    public GetUxmOrderDetailInfoRequest ToGetOrderDetailInfoReqData(
        GetOrderDetailByClientQuery query,
        string merchantNumber,
        string tradeNo,
        long timestamp)
    {
        var result = mapper.Map<GetUxmOrderDetailInfoRequest>(query);
        return result with {
            MerchantNumber = merchantNumber,
            TradeNo = tradeNo,
            Timestamp = timestamp
        };
    }
    
    public EstimateQuoteRequest ToGetEstimateQuoteReqData(
        EstimateQuoteCommand command,
        string merchantNumber,
        long timestamp)
    {
        var result = mapper.Map<EstimateQuoteRequest>(command);
        return result with {
            MerchantNumber = merchantNumber,
            Direction = OrderType.Of(command.OrderType).UxmValue,
            Timestamp = timestamp
        };
    }
}
