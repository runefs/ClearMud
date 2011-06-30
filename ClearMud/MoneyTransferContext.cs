using System;

namespace ClearMudSharp
{
    public class MoneyTransferContext : ClearMud<MoneyTransferContext>
    {
        [Role(Name = "Source")]
        public interface ISource
        {
            Func<Account,decimal,bool> TransferFrom { get; }
        }

        private static class RoleMethods
        {
            public static bool TransferFrom([Role]Account Source, Account destination, decimal amount)
            {
                Source.DecreaseBalance(amount);
                Source.Log("Withdrew: " + amount);
                destination.IncreaseBalance(amount);
                destination.Log("Withdrew: " + amount);
                return true;
            }
        }

        private ISource Source;
        private Account Destination;
        public void Doit(decimal amount)
        {
            Source.TransferFrom(Destination, amount);
        }

        private MoneyTransferContext()
        {
        }

        public static MoneyTransferContext Bind()
        {
            var _this = new MoneyTransferContext();
            Bind(_this,new {Source = new Account(),Destination = new Account()});
            return _this;
        }

    }
}