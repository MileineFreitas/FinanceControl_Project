namespace FinanceControl.Contracts.Dtos.Common;

public class DataResultDto<T>
{
    public int Page { get; set; }

    public int Total { get; set; }

    public List<T> Result { get; set; } = [];
}
