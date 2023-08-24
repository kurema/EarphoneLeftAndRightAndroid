﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EarphoneLeftAndRight.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DictionaryTabbed : TabbedPage
    {
        public DictionaryTabbed()
        {
            InitializeComponent();
        }

        private async void ToolbarItem_Clicked_TextToSpeech(object sender, EventArgs e)
        {
            var tts = DependencyService.Get<Dependency.ITextToSpeech>();
            if (tts.IsSpeaking) { await tts.Clear(); return; }
            if (this.SelectedItem is not ViewModels.DictionaryViewModel vm) return;
            var strings = Helper.Helpers.XhtmlToStrings(vm.Html);
            await tts.Clear();
            var sb=new StringBuilder();
            foreach (var item in strings)
            {
                var text = item;
                text = System.Text.RegularExpressions.Regex.Replace(text, @"\([^\)]*\)", "");
                text = System.Text.RegularExpressions.Regex.Replace(text, @"\[[^\]]*\]", "");
                sb.AppendLine(text);
            }
			var cultureInfo = new System.Globalization.CultureInfo(vm.HtmlLocale);
            await tts.SpeakAsync(sb.ToString(), new Dependency.TextToSpeechOptions() { Locale = new Dependency.TextToSpeechLocale(cultureInfo) });
		}
	}
}