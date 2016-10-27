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
    public class ScrumPokerStoryModel : ModelBase<ScrumPokerStory>
    {
        public string Name
        {
            get { return OriginalObject.StoryName; }
            set {
                ModifiedObject.StoryName = value;
                SetDirty();
            }
        }

    }
}
