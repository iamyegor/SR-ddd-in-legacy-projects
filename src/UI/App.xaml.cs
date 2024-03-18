using PackageDelivery.Delivery;

namespace PackageDelivery
{
    public partial class App
    {
        public App()
        {
            DBHelper.Init(@"Server=localhost;Database=PackageDelivery;User Id=packageDelivery;Password=pd;");
        }
    }
}
