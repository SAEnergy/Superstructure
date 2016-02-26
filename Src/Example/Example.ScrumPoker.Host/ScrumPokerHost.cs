using Core.Comm.BaseClasses;
using Example.ScrumPoker.Interfaces;

namespace Example.ScrumPoker.Host
{
    public class ScrumPokerHost : ServiceHostBase<IScrumPoker>, IScrumPoker
    {
        private static readonly double[] _cardValues = { 0, 0.5, 1, 2, 3, 5, 8, 13, 20, 40, 100, double.PositiveInfinity };

        double[] IScrumPoker.GetCardValues()
        {
            return _cardValues;
        }
    }
}
