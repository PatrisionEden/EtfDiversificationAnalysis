let initialData;
let IsinToPrice = new Map();
let IsinToTicker = new Map();

let diversificationModel = undefined;

google.charts.load('current', {'packages':['corechart']});
const divWithDiagram = document.getElementById("diagramContentItem");
const reloadButton = document.getElementById("reloadButton");
reloadButton.addEventListener("click", loadDiversificationModel);
google.charts.setOnLoadCallback(loadDiversificationModel);


async function loadDiversificationModel()
{
  const response = await fetch("/diversificationmodel", {
    method: "Get",
    headers: { "Accept": "application/json", "Content-Type": "application/json" },
  });
  dm = await response.json();
  diversificationModel = dm;
  console.log(dm);
  dm.CountriesArray = KeyValuePairsListToTwoDArray(dm.CountriesToPart);
  dm.SectorsArray = KeyValuePairsListToTwoDArray(dm.SectorToPart);
  dm.IndustriesArray = KeyValuePairsListToTwoDArray(dm.IndustryToPart);
  dm.IsinsArray = KeyValuePairsListToTwoDArray(dm.IsinToPart);
  console.log(dm.IsinToPart);

  drawDiagram('countriesDiversificationDiagram', dm.CountriesArray);
  drawDiagram('sectorsDiversificationDiagram', dm.SectorsArray);
  drawDiagram('industriesDiversificationDiagram', dm.IndustriesArray);
  drawDiagram('companiesDiversification', dm.IsinsArray);
};

function KeyValuePairsListToTwoDArray(keyValuePairsList)
{
  result = new Array();
  result.push([" ", " "]);
  keyValuePairsList.forEach(pair => {
    result.push([pair.Key, pair.Value]);
  })
  return result;
}

function drawDiagram(chartDivId, dataArray)
{
  var dataTable = google.visualization.arrayToDataTable(dataArray);
  var options = {
    chartArea: {
      height: '100%',
      widths: '100%',
      top: 100,
      left: 30,
      right: 30,
      bottom: 0
    },
    width: 500,
    height: 650,
    pieHole: 0.4,
    legend: {position: 'top', textStyle: {color: "#222", fontSize: 14}, maxLines:5}
  };

  var chart = new google.visualization.PieChart(document.getElementById(chartDivId));
  chart.draw(dataTable, options);
}










