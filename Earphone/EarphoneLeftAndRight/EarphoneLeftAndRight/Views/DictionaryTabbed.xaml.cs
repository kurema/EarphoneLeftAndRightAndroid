using System;
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

			var cultureInfo = new System.Globalization.CultureInfo(vm.HtmlLocale);
			var option = new Dependency.TextToSpeechOptions() { Locale = new Dependency.TextToSpeechLocale(cultureInfo) };
			//AndroidのTTSは文章が長すぎると読んでくれない。なので元々分割してあるのでそのまま流す。元の文章が適切な長さ以内でない場合は不適切。
			await tts.SpeakAsync(strings.Select(text =>
			{
				text = System.Text.RegularExpressions.Regex.Replace(text, @"\([^\)]*\)", "");
				text = System.Text.RegularExpressions.Regex.Replace(text, @"\[[^\]]*\]", "");
				return (text, option);
			}).ToArray());
		}
	}
}