using Newtonsoft.Json;
using ReceiptToJson.Model;

List<ReceiptSaaSResponse> receiptSaaSResponses = new List<ReceiptSaaSResponse>();
string json = String.Empty;

try
{
    //Root klasöründe yer alan response.json dosya yolu alınır
    string path = Environment.CurrentDirectory + @"\Root\response.json";
    using (StreamReader streamReader = new StreamReader(path))
    {
        json = streamReader.ReadToEnd();
    }
    if (!String.IsNullOrWhiteSpace(json))
    {
        //Fişin SaaS çıktı oluşturulan listeye Deserialize edilerek eklenir
        receiptSaaSResponses = JsonConvert.DeserializeObject<List<ReceiptSaaSResponse>>(json);
    }
}
catch (Exception ex)
{

    Console.WriteLine(ex.Message);
}



if (receiptSaaSResponses is not null)
{
    //Formatlanmış çıktı alınır
    List<FormattedReceiptResponse> receiptResponse = GetFormattedReceiptJson(receiptSaaSResponses);
    if (receiptResponse != null)
    {
        foreach (var item in receiptResponse)
        {
            Console.WriteLine(item.Line + " : " + item.Text);
        }
        Console.ReadLine();
    }
}

#region GetFormattedReceiptJson

List<FormattedReceiptResponse> GetFormattedReceiptJson(List<ReceiptSaaSResponse> receiptSaaSResponses)
{
    List<FormattedReceiptResponse> response = new();
    try
    {
        //En düşük satır yüksekliği bulunur
        int minRowHeight = int.MaxValue;
        int rowHeight = 0;
        for (int i = 1; i < receiptSaaSResponses.Count; i++)
        {
            //En düşük satır yüksekliği sol alt köşenin y kordinatından sol üst köşenin y kordinatı çıkarılarak bulunur.
            rowHeight = receiptSaaSResponses[i].boundingPoly.vertices[2].y - receiptSaaSResponses[i].boundingPoly.vertices[0].y;
            if (rowHeight < minRowHeight)
            {
                minRowHeight = rowHeight;
            }
        }

        int lastRowCoordinate = 0;
        int currentCoordinate = 0;
        int lineNumber = 1;
        //Json deserielaze edilerek elde edilen liste for döngüsünde dönülerek gerekli işlemler yapılır
        for (int i = 1; i < receiptSaaSResponses.Count; i++)
        {
            currentCoordinate = receiptSaaSResponses[i].boundingPoly.vertices[0].y;
            //Mevcut kordinat son kordinat ile minimum satır yükseliğinin toplamından büyükse yeni bir satıra geçtiği anlaşılır,
            //Yeni satıra geçmemişse response listesinin son elemanının description değişkenine ekleme yapılarak, yeni satıra geçmeden mevcut satıra kelime eklenir.
            if (currentCoordinate > lastRowCoordinate + minRowHeight)
            {
                response.Add(new FormattedReceiptResponse() { Line = lineNumber, Text = receiptSaaSResponses[i].description });
                lastRowCoordinate = currentCoordinate;
                lineNumber++;
            }
            else
            {
                var text = response.LastOrDefault().Text;
                text += " " + receiptSaaSResponses[i].description;
                response.LastOrDefault().Text = text;
            }

        }
    }
    catch (Exception)
    {

        throw;
    }
    return response;
}

#endregion


