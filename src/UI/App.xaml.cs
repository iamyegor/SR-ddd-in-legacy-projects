using PackageDelivery.Delivery;
using PackageDeliveryNew.Utils;

namespace PackageDelivery
{
    public partial class App
    {
        public App()
        {
            string connectionString =
                "Server=localhost;Database=PackageDelivery;User Id=packageDelivery;Password=pd;TrustServerCertificate=True;";

            DBHelper.Init(connectionString);
            ConnectionString.Set(connectionString);
        }
    }
}
