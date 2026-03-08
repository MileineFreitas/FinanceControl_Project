using System.Collections.Generic;
using FinanceControl.Domain.Entities.Transactions;

namespace FinanceControl.Domain.Interfaces.Repositories.Transactions;

public interface ITransactionRepository
{
    IEnumerable<Transaction> GetAllTransactions();

    Transaction GetTransactionById(int id);

    Transaction CreateTransaction(Transaction transaction);

    Transaction UpdateTransaction(Transaction transaction);

    Transaction DeleteTransaction(int id);
}
