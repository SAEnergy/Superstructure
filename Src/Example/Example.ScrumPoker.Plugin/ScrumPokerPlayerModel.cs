using Client.Base;
using Core.Comm.BaseClasses;
using Example.ScrumPoker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

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

        public byte Red { get { return OriginalObject.CardColor.Item1; } set { ModifiedObject.CardColor = new Tuple<byte, byte, byte>(value, ModifiedObject.CardColor.Item2, ModifiedObject.CardColor.Item3); SetDirty(); } }
        public byte Green { get { return OriginalObject.CardColor.Item2; } set { ModifiedObject.CardColor = new Tuple<byte, byte, byte>(ModifiedObject.CardColor.Item1, value, ModifiedObject.CardColor.Item3); SetDirty(); } }
        public byte Blue { get { return OriginalObject.CardColor.Item3; } set { ModifiedObject.CardColor = new Tuple<byte, byte, byte>(ModifiedObject.CardColor.Item1, ModifiedObject.CardColor.Item2, value); SetDirty(); } }

        public Brush BackgroundBrush
        {
            get { return new SolidColorBrush(Color.FromRgb(OriginalObject.CardColor.Item1, OriginalObject.CardColor.Item2, OriginalObject.CardColor.Item3)); }
        }

        public void SetVote(ScrumPokerCard card)
        {
            ModifiedObject.SelectedCard = card;
            ModifiedObject.HasVoted = true;
            SetDirty();
        }

        public ScrumPokerCard SelectedCard
        {
            get { return OriginalObject.SelectedCard; }
            set { SetVote(value); }
        }

        public ScrumPokerPlayerModel()
        {
            ScrumPokerSettings settings = ClientSettingsEngine.Instance.GetInstance<ScrumPokerSettings>();

            ScrumPokerPlayer player = new ScrumPokerPlayer();
            player.Name = settings.PlayerName;
            player.ID = Guid.NewGuid();
            player.CardColor = new Tuple<byte, byte, byte>(settings.ColorRed, settings.ColorGreen, settings.ColorBlue);
            // leave blank to let server auto assign
            //((byte)(_randy.Next(127) + 128), (byte)(_randy.Next(127) + 128), (byte)(_randy.Next(127) + 128));
            UpdateFrom(player);
        }

        public ScrumPokerPlayerModel(ScrumPokerPlayer player)
        {
            UpdateFrom(player);
        }
    }
}