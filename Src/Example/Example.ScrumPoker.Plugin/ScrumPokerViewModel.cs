using Client.Base;
using Core.Comm;
using Example.ScrumPoker.Interfaces;
using System;
using System.Collections.ObjectModel;

namespace Example.ScrumPoker.Plugin
{
    public class ScrumPokerViewModel : ViewModelBase<IScrumPoker>, IScrumPokerCallback
    {
        public ObservableCollection<double> CardValues { get; private set; }

        public ScrumPokerViewModel(ViewBase parent) : base(parent)
        {
            CardValues = new ObservableCollection<double>();
        }

        public void DummyCallback()
        {
        }

        protected override void OnConnect(ISubscription source)
        {
            base.OnConnect(source);
            foreach (double d in Channel.GetCardValues())
            {
                BeginInvoke(() => CardValues.Add(d));
            }
        }

        protected override void OnDisconnect(ISubscription source, Exception error)
        {
            base.OnDisconnect(source, error);
        }
    }
}
