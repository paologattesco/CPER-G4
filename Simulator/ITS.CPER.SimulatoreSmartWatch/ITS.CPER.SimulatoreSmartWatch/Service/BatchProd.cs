//using CsvHelper;
//using System.Formats.Asn1;
//using System.Globalization;

//namespace ITS.CPER.SimulatoreSmartWatch.Service
//{
//    public class BatchProd
//    {
//        public int Guid { get; set; }
//        public string Date { get; set; }

//        public void CvsFile()
//        {
//            using (var streamWriter = new StreamWriter("ProductionBatch.csv"))
//            {
//                using (var cvsWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
//                {
//                    var productionBatch = new List<BatchProd>()
//                    {
//                    new BatchProd { Guid = 1, Date = "18/12/2002"},
//                    new BatchProd { Guid = 2, Date = "18/12/2002"},
//                    new BatchProd { Guid = 3, Date = "18/12/2002"}
//                    };
//                    cvsWriter.WriteRecords(productionBatch);
//                }
//            }
//        }
//    }
//}
