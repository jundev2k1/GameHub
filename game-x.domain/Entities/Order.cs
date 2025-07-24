namespace game_x.domain.Entities;

public sealed class Order : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public string OrderUid { get; set; } = string.Empty;
    public OrderType OrderType { get; private set; } = default!;
    public string UserId { get; private set; } = string.Empty;
    public AppUser User { get; private set; } = default!;
    public int CounterId { get; private set; } = default!;
    public Counter Counter { get; private set; } = default!;
    public string StaffId { get; private set; } = default!;
    public AppUser Staff { get; private set; } = default!;
    public PricingMode PricingMode { get; private set; } = PricingMode.FiatAmountFixed;
    public FiatType? FiatType { get; private set; }
    public CryptoType? CryptoType { get; private set; }
    public decimal FiatAmount { get; private set; }
    public decimal CryptoAmount { get; private set; }
    public int Quantity { get; private set; }
    public decimal? PricePerUnit { get; private set; }
    public decimal TotalPrice { get; private set; }
    public CurrencyUnit? CurrencyUnit { get; private set; }
    public OrderStatus OrderStatus { get; private set; } = default!;
    public decimal Fee { get; private set; }
    public string Notes { get; private set; } = string.Empty;
    public string? Metadata { get; private set; }
    public long UxmTimestamp { get; private set; }
    public string PayeeBankName { get; private set; } = string.Empty;
    public string PayeeBranchCode { get; private set; } = string.Empty;
    public string PayeeName { get; private set; } = string.Empty;
    public string PayeeAccountNumber { get; private set; } = string.Empty;
    public string? EntryCode { get; private set; }

    public static Order Create(
        OrderType type,
        string userId,
        int counterId,
        string staffId,
        OrderStatus status,
        decimal? pricePerUnit = null,
        CurrencyUnit? currencyUnit = null,
        FiatType? fiatType = null,
        CryptoType? cryptoType = null,
        PricingMode? pricingMode = null,
        decimal? fiatAmount = null,
        decimal? cryptoAmount = null,
        int? quantity = null,
        decimal? fee = null,
        string? notes = null,
        string? metadata = null,
        string? entryCode = null,
        string? payeeBankName = null,
        string? payeeBranchCode = null,
        string? payeeName = null,
        string? payeeAccountNumber = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));
        ArgumentException.ThrowIfNullOrWhiteSpace(staffId, nameof(staffId));

        if (quantity != null && quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        if (pricePerUnit != null && pricePerUnit < 0)
            throw new ArgumentException("PricePerUnit must be greater than zero.", nameof(pricePerUnit));
        if (fiatAmount != null && fiatAmount < 0)
            throw new ArgumentException("FiatAmount must be greater than zero.", nameof(fiatAmount));
        if (cryptoAmount != null && cryptoAmount < 0)
            throw new ArgumentException("CryptoAmount must be greater than zero.", nameof(cryptoAmount));
        if (fee != null && fee < 0)
            throw new ArgumentException("Fee must be greater than zero.", nameof(fee));

        var order = new Order
        {
            OrderType = type ?? throw new ArgumentNullException(nameof(type)),
            UserId = userId,
            CounterId = counterId,
            StaffId = staffId,
            FiatType = fiatType ?? Enum.FiatType.Cny,
            CryptoType = cryptoType ?? Enum.CryptoType.Trc20Usdt,
            PricingMode = pricingMode ?? PricingMode.FiatAmountFixed,
            FiatAmount = fiatAmount ?? 0,
            CryptoAmount = cryptoAmount ?? 0,
            Fee = fee ?? 0,
            Quantity = quantity ?? 0,
            PricePerUnit = pricePerUnit ?? 0,
            TotalPrice = (pricePerUnit ?? 0) * (quantity ?? 0),
            CurrencyUnit = currencyUnit ?? CurrencyUnit.Of(CurrencyCode.CNY),
            OrderStatus = status,
            Notes = notes ?? "",
            Metadata = metadata,
            EntryCode = entryCode,
            PayeeBankName = payeeBankName ?? "",
            PayeeBranchCode = payeeBranchCode ?? "",
            PayeeName = payeeName ?? "",
            PayeeAccountNumber = payeeAccountNumber ?? "",
        };
        return order;
    }

    public void Update(
        int quantity,
        decimal priceOfUnit,
        CurrencyUnit currencyUnit,
        OrderStatus status,
        string notes)
    {
        Quantity = quantity;
        PricePerUnit = priceOfUnit;
        CurrencyUnit = currencyUnit;
        OrderStatus = status;
        Notes = notes;
    }

    public void UpdateOrderUid(string orderUid)
    {
        ArgumentException.ThrowIfNullOrEmpty(orderUid, nameof(OrderUid));

        OrderUid = orderUid;
    }

    public void UpdateMetadata(string? metadata)
    {
        var isValid = metadata.IsNullOrEmpty()
            || JsonHelper.IsJson(metadata.ToStringOrEmpty());
        if (!isValid) throw new ArgumentException("Metadata must be of type json.");

        Metadata = metadata;
    }

    public void UpdateStatus(OrderStatus status)
    {
        OrderStatus = status;
    }

    public void UpdateUxmOrder(string orderUid, string entryCode, decimal fiatAmount, decimal cryptoAmount, decimal fee, long timestamp)
    {
        UpdateOrderUid(orderUid);

        EntryCode = entryCode;
        FiatAmount = fiatAmount;
        CryptoAmount = cryptoAmount;
        Fee = fee;
        UxmTimestamp = timestamp;
    }
}
