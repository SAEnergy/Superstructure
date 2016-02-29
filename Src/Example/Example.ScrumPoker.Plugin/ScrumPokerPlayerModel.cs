using Client.Base;
using Core.Comm.BaseClasses;
using Example.ScrumPoker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.ScrumPoker.Plugin
{
    public class ScrumPokerPlayerModel : ModelBase<ScrumPokerPlayer>
    {
        public string Name
        {
            get { return OriginalObject.Name; }
            set
            {
                ModifiedObject.Name = value;
                SetDirty();
            }
        }

        public void SetVote(ScrumPokerCard card)
        {
            ModifiedObject.SelectedCard = card;
            ModifiedObject.HasVoted = true;
            SetDirty();
        }

        public void ResetVote()
        {
            ModifiedObject.SelectedCard = null;
            ModifiedObject.HasVoted = false;
            SetDirty();
        }

        public ScrumPokerCard SelectedCard
        {
            get { return OriginalObject.SelectedCard; }
            set { SetVote(value); }
        }

        public ScrumPokerPlayerModel() { }
        public ScrumPokerPlayerModel(ScrumPokerPlayer player)
        {
            UpdateFrom(player);
        }
    }
}