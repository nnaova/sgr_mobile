using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SGR_Mobile.Vues
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NavAdmin : TabbedPage
	{
		public NavAdmin ()
		{
			InitializeComponent ();
		}
        protected override void OnAppearing()
        {
            base.OnAppearing();
			CurrentPage = acceuilAdminPage;
        }
    }
}