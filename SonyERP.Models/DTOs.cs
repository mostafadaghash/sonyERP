namespace SonyERP.Models;

public record SaleRequestDTO(
    string GameName,
    Platform Platform,
    CopyType CopyType,
    decimal Price,
    int EmployeeUserId,
    int CustomerId,
    int BranchId
);

public record SaleResultDTO(
    bool Success,
    string Message,
    int? SaleId = null,
    int? GameAccountId = null
);
