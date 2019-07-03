using IgWpfWorkshop.ViewModel;
using Infragistics.Samples.Data.Models;
using Infragistics.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IgWpfWorkshop
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        public Dashboard()
        {
            InitializeComponent();
            this.revealView1.DataSourcesRequested
                += RevealView1_DataSourcesRequested;

            this.revealView1.SaveDashboard += RevealView1_SaveDashboard;

            //Data Source Provide
            this.revealView1.DataSourceProvider =
                new SampleDataSourceProvider();
            //Data Provider
            this.revealView1.DataProvider =
                new EmbedDataProvider(this.DataContext as DashboardViewModel);
            //新規作成のため、設定なし。
            this.revealView1.Settings =
                new RevealSettings(null);
        }
        private void RevealView1_DataSourcesRequested(
            object sender, DataSourcesRequestedEventArgs e)
        {
            var inMemoryDSI = new RVInMemoryDataSourceItem(
                "SalesAmountByProductData");
            inMemoryDSI.Title = "SalesAmountByProductData";
            inMemoryDSI.Description = "SalesAmountByProductData";



            var inMemoryDSI2 = new RVInMemoryDataSourceItem("SalesTargetThisYear");
            inMemoryDSI2.Title = "SalesTargetThisYear";
            inMemoryDSI2.Description = "SalesTargetThisYear";


            var inMemoryDSI3 = new RVInMemoryDataSourceItem("TotalSalesThisYear");
            inMemoryDSI3.Title = "TotalSalesThisYear";
            inMemoryDSI3.Description = "TotalSalesThisYear";

            var inMemoryDSI4 = new RVInMemoryDataSourceItem("Top30LargeDeals");
            inMemoryDSI4.Title = "Top30LargeDeals";
            inMemoryDSI4.Description = "Top30LargeDeals";

            var inMemoryDSI5 = new RVInMemoryDataSourceItem("MonthlySalesAmount");
            inMemoryDSI5.Title = "MonthlySalesAmount";
            inMemoryDSI5.Description = "MonthlySalesAmount";


            e.Callback(new RevealDataSources(
                null,
                         new List<object>() { inMemoryDSI, inMemoryDSI2, inMemoryDSI3, inMemoryDSI4, inMemoryDSI5 },
                    false));
        }

        private async void RevealView1_SaveDashboard(object sender, DashboardSaveEventArgs args)
        {
            //ファイルの保存
            var data = await args.Serialize();
            using (var output = File.OpenWrite($"{args.Name}.rdash"))
            {
                output.Write(data, 0, data.Length);
            }
            args.SaveFinished();
        }
    }


    public class SampleDataSourceProvider : IRVDataSourceProvider
    {
        public Task<RVDataSourceItem> ChangeDashboardFilterDataSourceItemAsync(
             RVDashboardFilter globalFilter, RVDataSourceItem dataSourceItem)
        {
            return Task.FromResult<RVDataSourceItem>(null);
        }

        public Task<RVDataSourceItem> ChangeVisualizationDataSourceItemAsync(
             RVVisualization visualization, RVDataSourceItem dataSourceItem)
        {

            var csvDsi = dataSourceItem as RVCsvDataSourceItem;
            if (csvDsi != null)
            {
                var inMemDsi = new RVInMemoryDataSourceItem(csvDsi.Id);

                return Task.FromResult((RVDataSourceItem)inMemDsi);
            }
            return Task.FromResult((RVDataSourceItem)null);

        }


    }

    public class EmbedDataProvider : IRVDataProvider
    {
        private DashboardViewModel vm;
        public EmbedDataProvider(DashboardViewModel _vm)
        {
            vm = _vm;
        }

        public Task<IRVInMemoryData> GetData(
            RVInMemoryDataSourceItem dataSourceItem)
        {
            var datasetId = dataSourceItem.DatasetId;
            if (datasetId == "SalesAmountByProductData")
            {
                var data = vm.SalesAmountByProductData.ToList<SalesAmountByProduct>();

                return Task.FromResult<IRVInMemoryData>(new RVInMemoryData<SalesAmountByProduct>(data));
            }
            if (datasetId == "SalesTargetThisYear")
            {
                var data = vm.SalesTargetThisYear;

                return Task.FromResult<IRVInMemoryData>(new RVInMemoryData<int>(new List<int> { data }));
            }
            if (datasetId == "TotalSalesThisYear")
            {
                var data = vm.TotalSalesThisYear;

                return Task.FromResult<IRVInMemoryData>(new RVInMemoryData<int>(new List<int> { data }));
            }
            if (datasetId == "Top30LargeDeals")
            {
                var data = vm.Top30LargeDeals.ToList<Sale>();

                return Task.FromResult<IRVInMemoryData>(new RVInMemoryData<Sale>(data));
            }
            if (datasetId == "MonthlySalesAmount")
            {
                var data = vm.MonthlySalesAmount.ToList<MonthlySale>();

                return Task.FromResult<IRVInMemoryData>(new RVInMemoryData<MonthlySale>(data));
            }

            else
            {
                throw new Exception("Invalid data requested");
            }
        }
    }

   
}