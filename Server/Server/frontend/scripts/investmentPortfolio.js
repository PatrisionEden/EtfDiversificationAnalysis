const investmentPortfolio = document.querySelector(".investmentPortfolio");
const addSecurityButton = document.getElementById("addSecurityButton");
const selectDialog = document.getElementById("SelectDialog");
const securitySelect = document.getElementById("securitySelect");
const closeSelectDialogButton = document.getElementById("closeSelectDialogButton");
const SelectDialogConfirmButton = document.getElementById("SelectDialogConfirmButton");
const saveSecuritiesListButton = document.getElementById("saveSecuritiesListButton");
let userInvestmentPortfolio;
let avilableEtfs;

addSecurityButton.addEventListener("click", ShowSelectDialog);
closeSelectDialogButton.addEventListener("click", HideSelectDialog);
SelectDialogConfirmButton.addEventListener("click", SelectDialogConfirmButtonAction);
saveSecuritiesListButton.addEventListener("click", SaveSecuritiesList);


async function LoadUserInvetsmentPortfolio(){
   const response = await fetch("/getinvestmentportfolio");
   userInvestmentPortfolio = await response.json();
   console.log(userInvestmentPortfolio);
};

async function LoadAvailableEtfs(){
   const response = await fetch("/getavailableetfs");
   avilableEtfs = await response.json();
   console.log(avilableEtfs);
};

//getavailableetfs

function ShowInvestmentPortfolio()
{
   investmentPortfolio.style.display = "block";
};

function ShowSelectDialog()
{
   selectDialog.style.display = "flex";
};

function HideSelectDialog()
{
   selectDialog.style.display = "none";
};

function SelectDialogConfirmButtonAction()
{
   let selectedOption = securitySelect.selectedOptions[0];
   let selectedIsin = selectedOption.value;

   let etf = avilableEtfs.find(element => element.Isin == selectedIsin);
   console.log(etf);

   InsertItemInSecurities(etf.Ticker, 0, etf.Price, etf.Isin);
   HideSelectDialog();
};

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
   //console.log(initialData.UserData.Portfolio);

   const response = await fetch("/saveinvestmentportfolio", {
      method: "POST",
      headers: { "Accept": "application/json", "Content-Type": "application/json" },
      body: JSON.stringify(Portfolio)
  });
};

async function LoadInvestmentPortfolio()
{
   let securitiesList = document.getElementById("securitiesList");
   let ul = securitiesList.firstElementChild;
   ul.innerHTML = "";
   let portfolio = userInvestmentPortfolio;
   portfolio.forEach(element => {
      InsertItemInSecurities(element.Ticker, element.Amount, element.Price, element.Isin);
   });
};

function InsertItemInSecurities(tickerStr, amount, price, isin)
{
   let securitiesList = document.getElementById("securitiesList");
   let ul = securitiesList.firstElementChild;
   let li = document.createElement("li");
   let numberFormat = Intl.NumberFormat('en', {minimumFractionDigits: 2, maximumFractionDigits: 2});
   li.innerHTML =
      "<span class=\"securityName\">" + tickerStr + "</span>" +
      "<span class=\"securityAmount\">"+
         "<input type=\"number\" placeholder=\"amount\" value=\"" + amount + "\">"+
      "</span>" +
      "<span class=\"securityPrice\">" + numberFormat.format(price) + "</span>" +
      "<span class=\"securityTotal\">" + numberFormat.format(amount * price) + "</span>" +
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
};

function DeleteButtonAction(e)
{
   e.target.parentElement.remove();
};

function InitialiseSelectList()
{
   securitySelect.innerHTML="";
   avilableEtfs.forEach(element => {
      let option = document.createElement("option");
      option.innerHTML = element.Ticker;
      option.value = element.Isin;
      securitySelect.append(option);
   });
   securitySelect.firstElementChild.selected = true;
};

async function Main(){
   await LoadUserInvetsmentPortfolio();
   await LoadAvailableEtfs();
   LoadInvestmentPortfolio();
   ShowInvestmentPortfolio();
   InitialiseSelectList();
}
Main();