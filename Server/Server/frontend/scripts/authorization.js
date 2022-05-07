const headerLoginButton = document.getElementById("headerLoginButton");
const authorization = document.getElementById("authorizationDialog");
const authorizationLoginButton = document.getElementById("authorizationLoginButton");
const userNameButton = document.getElementById("userNameButton");
const registrationButton = document.getElementById("registrationButton");
const signInH2 = document.getElementById("signIn");
const signUpH2 = document.getElementById("signUp");
const closeAuthorizationButton = document.getElementById("closeAuthorizationButton");

headerLoginButton.addEventListener("click", ShowAuthorizationDiv);
closeAuthorizationButton.addEventListener("click", HideAuthorizationDiv);
signInH2.addEventListener("click", SwitchToSignIn);
signUpH2.addEventListener("click", SwitchToSignUp);
authorizationLoginButton.addEventListener("click", AthorizationAjax);
registrationButton.addEventListener("click", RegistrationAjax);

function ShowAuthorizationDiv()
{
   authorization.style.display = "flex";
};

function HideAuthorizationDiv()
{
   authorization.style.display = "none";
};

function SwitchToSignIn(){
   signInH2.classList.add("selected");
   signUpH2.classList.remove("selected");

   document.getElementById("registrationForm").style.display = "none";
   document.getElementById("loginForm").style.display = "flex";
};

function SwitchToSignUp(){
   signUpH2.classList.add("selected");
   signInH2.classList.remove("selected");

   document.getElementById("loginForm").style.display = "none";
   document.getElementById("registrationForm").style.display = "flex";
};

function SetAndShowUserName(userName){
   userNameButton.style.display = "block";
   userNameButton.innerHTML = userName;
};

async function RegistrationAjax(e)
{
   e.preventDefault();
   const response = await fetch("/registration", {
      method: "POST",
      headers: { "Accept": "application/json", "Content-Type": "application/json" },
      body: JSON.stringify({
          login: document.getElementById("registrationLogin").value,
          password: document.getElementById("registrationPassword").value
      })
  });
  initialData = await response.json();
  IsinToPriceInitialization();
  IsinToTickerInitialization();
  SetAndShowUserName(initialData.UserData.UserName);
  HideAuthorizationDiv();
  LoadInvestmentPortfolio();
  InitialiseSelectList();
};

async function AthorizationAjax(e)
{
   e.preventDefault();
   const response = await fetch("/authorization", {
      method: "POST",
      headers: { "Accept": "application/json", "Content-Type": "application/json" },
      body: JSON.stringify({
          login: document.getElementById("userAuthorizationLogin").value,
          password: document.getElementById("userAuthorizationPassword").value
      })
  });
  initialData = await response.json();
  IsinToPriceInitialization();
  IsinToTickerInitialization();
  SetAndShowUserName(initialData.UserData.UserName);
  HideAuthorizationDiv();
  LoadInvestmentPortfolio();
  InitialiseSelectList();
};

function IsinToPriceInitialization()
{
   IsinToPrice = new Map();
   initialData.IsinToPrice.forEach(element => {
      IsinToPrice.set(element.Key, element.Value);
   });
}

function IsinToTickerInitialization()
{
   IsinToTicker = new Map();
   initialData.IsinToTicker.forEach(element => {
      IsinToTicker.set(element.Key, element.Value);
   });
}