﻿using System.Windows.Input;

namespace DevToolVault.Features.Start
{
    public class CardItem
    {
        public string Icon { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ICommand OpenCommand { get; set; }
    }
}
