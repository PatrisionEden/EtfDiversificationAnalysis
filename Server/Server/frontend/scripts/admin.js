const ul = document.getElementById("securitiesList").firstElementChild;
const reportDialog = document.getElementById("ReportDialog");
const closeSelectDialogButton = document.getElementById("closeSelectDialogButton");

closeSelectDialogButton.addEventListener("click", () => reportDialog.style.display = "none");

LoadReports();

async function LoadReports(){
   const response = await fetch("/getreports");
   let reports = await response.json();
   console.log(reports);

   reports.forEach(element => {
      let li = document.createElement("li");
      li.classList.add("securitiesList-item");

      let span = document.createElement("span");
      span.classList.add("reportId");
      span.innerHTML = element.ReportId;
      li.appendChild(span);
      span = document.createElement("span");
      span.classList.add("sender");
      span.innerHTML = element.Sender;
      li.appendChild(span);
      span = document.createElement("span");
      span.classList.add("security");
      span.innerHTML = element.Isin;
      li.appendChild(span);
      span = document.createElement("span");
      span.classList.add("reportText");
      span.innerHTML = element.ReportText;
      li.appendChild(span);

      li.addEventListener("click", ShowReportDialog);

      ul.appendChild(li);
   });
}

function ShowReportDialog(event){
   let li = event.target;
   if(li.nodeName == "SPAN"){
      li = li.parentElement;
   }
   console.log(li);
   document.getElementById("reportHeader").innerHTML 
      = "Заявление о неточности №" +
      li.children[0].innerHTML;

   document.getElementById("IsinField").innerHTML 
   = "Isin: " +
   li.children[2].innerHTML;

   document.getElementById("reportText").innerHTML 
   = li.children[3].innerHTML;

   reportDialog.style.display = "flex";
};

