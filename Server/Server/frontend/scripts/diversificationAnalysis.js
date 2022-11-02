let diversificationModel = undefined;
let IsinToTicker = new Map();
let IsinToName = new Map();
let contentProvider = {
  Countries:{
    h2Content: 'Распределение по странам',
    route: 'CountriesToPart',
    headers: ["Страна", "Доля"]
  },
  Industries:{
    h2Content: 'Распределение по отраслям',
    route: 'IndustryToPart',
    headers: ["Отрасль", "Доля"]
  },
  Sectors:{
    h2Content: 'Распределение по секторам',
    route: 'SectorToPart',
    headers: ["Сектор", "Доля"]
  },
  Companies:{
    h2Content: 'Распределение по компаниям',
    route: 'IsinToPart',
    headers: ["Isin", "Доля", "Название", "Тикер"]
  },
}

google.charts.load('current', {'packages':['corechart']});
const divWithDiagram = document.getElementById("diagramContentItem");
const diversificationAnalysis = document.querySelector(".diversificationAnalysis");

DiversificationModelInit();

async function DiversificationModelInit(){
  await loadDiversificationModel();

  let section = GetSection();
  document.getElementById("diversificationLabel").firstElementChild.innerHTML = contentProvider[section].h2Content;
  let dataArr = KeyValuePairsListToTwoDArray(diversificationModel[contentProvider[section].route]);
  console.log(dataArr);
  if(section == 'Companies'){
    let tempArr = [];
    tempArr.push([' ', ' ']);
    dataArr.forEach(elem => {
      if(elem[0] != ' ' && IsinToTicker.get(elem[0]) != '')
        tempArr.push([IsinToTicker.get(elem[0]), elem[1]]);
    });
    dataArr = tempArr;
  }
  console.log(dataArr);

  drawDiagram(dataArr);
  DrawTable(section)
  
}

function DrawTable(section)
{
  if(section == 'Companies')
    {
      let dataArr = diversificationModel[contentProvider[section].route];
      console.log("aaaaaa123");
      console.log(dataArr);
      dataArr.forEach(elem => {
        elem.Name = IsinToName.get(elem.Key);
        elem.Ticker = IsinToTicker.get(elem.Key);
      });
      console.log(dataArr);
    }

    drawTable("Подробная информация", diversificationModel[contentProvider[section].route], contentProvider[section].headers);
}

function ShowDivesificationAnalysis()
{
  //console.log("aaaa");
  diversificationAnalysis.style.display = "block";
}

async function loadDiversificationModel()
{
  ShowDivesificationAnalysis();

  const response = await fetch("/diversificationmodel", {
    method: "Get",
    headers: { "Accept": "application/json", "Content-Type": "application/json" },
  });
  dm = await response.json();
  diversificationModel = dm;

  IsinToTicker = new Map();
  IsinToName = new Map();

  diversificationModel.IsinToTicker.forEach(element => {
    IsinToTicker.set(element.Key, element.Value);
  });
  diversificationModel.IsinToName.forEach(element => {
    IsinToName.set(element.Key, element.Value);
  });
};

function GetSection(){
  let searchUrl = new URLSearchParams(location.search);

  let section = searchUrl.get('section');

  if(section == 'Countries' || section == 'Industries' || section == 'Sectors' || section == 'Companies')
    return section;
  else
    return 'Countries';
}

function KeyValuePairsListToTwoDArray(keyValuePairsList)
{
  result = new Array();
  result.push([" ", " "]);
  keyValuePairsList.forEach(pair => {
    result.push([pair.Key, pair.Value]);
  })
  return result;
}

function drawDiagram(dataArray)
{
  console.log("start drawing");
  var dataTable = google.visualization.arrayToDataTable(dataArray);
  var options = {
    titleTextStyle: {
      fontSize: 33
    },
    chartArea: {
      height: '100%',
      widths: '100%',
      // top: 100,
      left: 10,
      right: 10,
      bottom: 0
    },
    width: 300,
    height: 300,
    pieHole: 0.4,
    legend: 'none'
  };

  var chart = new google.visualization.PieChart(document.getElementById("diversificationDiagram"));
  chart.draw(dataTable, options);
}

function drawTable(tableLabel, dataArr, headers){
  let numberFormat = Intl.NumberFormat('en', {minimumFractionDigits: 2, maximumFractionDigits: 2});
  let tableDiv = document.getElementById("diversificationTable");
  tableDiv.querySelector("h3").innerHTML=tableLabel;


  let table = tableDiv.querySelector("table");
  table.innerHTML = "";

  let headerRow = document.createElement("tr");
  headerRow.classList.add("diversificationTable-headerrow");

  headers.forEach(elem => {
    let cell = document.createElement("th");
    cell.innerHTML = elem;
    headerRow.appendChild(cell);
  });

  table.appendChild(headerRow);

  console.log(dataArr);

  dataArr.forEach(elem => {
    let row = document.createElement("tr");
    headerRow.classList.add("diversificationTable-row");

    for (const field in elem) {
      let cell = document.createElement("td");
      cell.innerHTML = elem[field];

      if(field == "Value")
      {
        cell.innerHTML = numberFormat.format(elem[field]);
      }

      row.appendChild(cell);
    }

    table.appendChild(row);
  });
}