let initialData = 
{
   UserName: "DEV version 0.01",
   AvailableETFs: ["AAAA", 'BBBB', "CCCC"]
};
let IsinToPrice = new Map();
let IsinToTicker = new Map();

const investmentPortfolio = document.querySelector(".investmentPortfolio");


async function LoadInvestmentPortfolio()
{
   ShowInvestmentPortfolio();
   let securitiesList = document.getElementById("securitiesList");
   let ul = securitiesList.firstElementChild;
   ul.innerHTML="";
   let portfolio = initialData.UserData.Portfolio;
   portfolio.forEach(element => {
      InsertItemInSecurities(IsinToTicker.get(element.Isin), element.Amount, IsinToPrice.get(element.Isin), element.Isin);
   });
};

function ShowInvestmentPortfolio()
{
   investmentPortfolio.style.display = "block";
}

const addSecurityButton = document.getElementById("addSecurityButton");
addSecurityButton.addEventListener("click", addNewItemInSecuritiesList);

const selectDialog = document.getElementById("SelectDialog");

function addNewItemInSecuritiesList()
{
   ShowSelectDialog();
};

function InsertItemInSecurities(tickerStr, amount, price, isin)
{
   let securitiesList = document.getElementById("securitiesList");
   let ul = securitiesList.firstElementChild;
   let li = document.createElement("li");
   li.innerHTML = 
   "<span class=\"securityName\">" + tickerStr + "</span>" +
   "<span class=\"securityAmount\">"+
      "<input type=\"number\" placeholder=\"amount\" value=\"" + amount + "\">"+
   "</span>" +
   "<span class=\"securityPrice\">" + price + "</span>" +
   "<span class=\"securityTotal\">" + amount * price + "</span>" +
   "<span class=\"securityIsin\" style=\"display: none;\">" + isin + "</span>" +
   "<img src=\"img/delete.png\" alt=\"\">"
   let deleteButton = li.querySelector("img");
   deleteButton.addEventListener("click", DeleteButtonAction);
   let amountInput = li.querySelector(".securityAmount");
   amountInput.addEventListener("input", function(e){
      let total = e.target.parentElement.parentElement.querySelector(".securityTotal");
      let price = e.target.parentElement.parentElement.querySelector(".securityPrice");
      console.log(total);
      total.innerHTML = e.target.value * price.innerHTML;
   });
   ul.append(li);
}

function DeleteButtonAction(e)
{
   e.target.parentElement.remove();
};

const securitySelect = document.getElementById("securitySelect");

function InitialiseSelectList()
{
   securitySelect.innerHTML="";
   for (let entry of IsinToTicker)
   {
      let option = document.createElement("option");
      option.innerHTML = entry[0] + " " + entry[1];
      securitySelect.append(option);
   };
   securitySelect.firstElementChild.selected = true;
}


const closeSelectDialogButton = document.getElementById("closeSelectDialogButton");
closeSelectDialogButton.addEventListener("click", HideSelectDialog);

function HideSelectDialog()
{
   selectDialog.style.display = "none";
};
function ShowSelectDialog()
{
   selectDialog.style.display = "flex";
};

const SelectDialogConfirmButton = document.getElementById("SelectDialogConfirmButton");
SelectDialogConfirmButton.addEventListener("click", SelectDialogConfirmButtonAction);

function SelectDialogConfirmButtonAction()
{

   let selectedOption = securitySelect.selectedOptions[0].innerHTML;
   let selectedIsin = selectedOption.split(' ')[0];

   InsertItemInSecurities(IsinToTicker.get(selectedIsin), 0, IsinToPrice.get(selectedIsin), selectedIsin);
   HideSelectDialog();
}

const saveSecuritiesListButton = document.getElementById("saveSecuritiesListButton");
saveSecuritiesListButton.addEventListener("click", SaveSecuritiesList);

async function SaveSecuritiesList()
{
   let securitiesList = document.getElementById("securitiesList");
   let ul = securitiesList.firstElementChild;
   let Portfolio = new Array();
   for (var i = 0; i < ul.children.length; i++){
      let security = {
         Isin: ul.children[i].querySelector(".securityIsin").innerHTML,
         Amount: Number(ul.children[i].querySelector(".securityAmount").firstElementChild.value)
      };
      Portfolio.push(security);
   };
   console.log(Portfolio);
   console.log(initialData.UserData.Portfolio);

   const response = await fetch("/saveinvestmentportfolio", {
      method: "POST",
      headers: { "Accept": "application/json", "Content-Type": "application/json" },
      body: JSON.stringify(Portfolio)
  });
};