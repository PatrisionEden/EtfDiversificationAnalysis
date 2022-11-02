const isinField = document.getElementById("IsinField");
const reportDialog = document.getElementById("ReportDialog");
const closeSelectDialogButton = document.getElementById("closeSelectDialogButton");
const selectDialogConfirmButton = document.getElementById("SelectDialogConfirmButton");
const isinHidden = document.getElementById("isinHidden");

closeSelectDialogButton.addEventListener("click", () => {reportDialog.style.display = "none";})
selectDialogConfirmButton.addEventListener("click", SendReport);

loadFullCompositionModelModel();
let fullCompositionModel;
let IsinToPart;

async function loadFullCompositionModelModel()
{
  const response = await fetch("/getfullcomposition", {
    method: "Get",
    headers: { "Accept": "application/json", "Content-Type": "application/json" },
  });
  let fcm = await response.json();
  fullCompositionModel = fcm;
  IsinToPart = new Map();

  fullCompositionModel.IsinToPart.forEach(element => {
   IsinToPart.set(element.Key, element.Value);
  });

  console.log(fullCompositionModel);
  console.log(IsinToPart);

  console.log("2");
  initTable();
};

function initTable(){
   let table = document.getElementById("fullCompositionTable");
   let numberFormat = Intl.NumberFormat('en', {minimumFractionDigits: 2, maximumFractionDigits: 2});

   fullCompositionModel.Shares.forEach(elem => {
      let row = document.createElement("tr");
      
      for (const field in elem) {
         let cell = document.createElement("td");
         cell.innerHTML = elem[field];
   
         row.appendChild(cell);
      }
      let cell = document.createElement("td");
      cell.innerHTML = numberFormat.format(IsinToPart.get(elem.Isin));
      row.appendChild(cell);

      row.addEventListener("click", ShowReportDialog);

      table.appendChild(row);
   });
}

function ShowReportDialog(event)
{
   isinHidden.innerHTML = event.target.parentElement.children[3].innerHTML;
   isinField.innerHTML = "Isin: " + event.target.parentElement.children[3].innerHTML;
   console.log(event.target.parentElement.children[3]);
   reportDialog.style.display = "flex";
};

async function SendReport(event){
   let text = document.getElementById("reportText").value;
   let isin = isinHidden.innerHTML;
   let arr = [isin, text];
   console.log(arr);
   const response = await fetch("/sendreport", {
      method: "POST",
      headers: { "Accept": "application/json", "Content-Type": "application/json" },
      body: JSON.stringify(arr)
  });
  
  reportDialog.style.display = "none";
}