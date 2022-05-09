using EtfParser;

FinexFactory factory = new FinexFactory();
var finexData = factory.CreateEtfProviderData();
factory.InitialiseEtfProviderData(finexData);
DBUpdater dBUpdater = new DBUpdater("../../../../db1.db");
dBUpdater.SaveOrUpdateIEtfProviderData(finexData);
Console.WriteLine("----FinEx----");
Console.WriteLine("Composite:");
foreach(var etfData in finexData.EtfDatas)
{
    Console.Write("\r" + etfData.EtfName);
}
Console.WriteLine();
Console.WriteLine();
foreach (var etfData in finexData.EtfDatas)
{
    if(etfData.IsInitialised == false)
        factory.InitialiseEtfData(etfData);
    dBUpdater.SaveOrUpdateIEtfData(etfData);
    Console.WriteLine(etfData.EtfName + "\t Amount of shares: " + etfData.ShareDatas.Count);
}
foreach (var etfData in finexData.EtfDatas)
{
    if (etfData.IsInitialised == false)
        factory.InitialiseEtfData(etfData);
    int count = 0;
    Console.WriteLine("Saving " + etfData.EtfName + ".");
    foreach(var shareData in etfData.ShareDatas)
    {
        Console.Write("\t\t\t\t\t\t\r");
        if(dBUpdater.IsShareDataRowExists(shareData.Isin) == false)
        {
            if (shareData.IsInitialised == false)
                factory.InitialiseShareData(shareData);
            dBUpdater.SaveOrUpdateIShareDataByIsin(shareData, false);
        }
        Console.Write("Saved "+ count + " from " + etfData.ShareDatas.Count);
        count++;
    }
    if (etfData.IsInitialised == false)
        factory.InitialiseEtfData(etfData);
    Console.WriteLine(etfData.EtfName + "\t Amount of shares: " + etfData.ShareDatas.Count);
}