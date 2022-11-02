const investmentPortfolioButton = document.getElementById("investmentPortfolioButton");
const headerLogo = document.getElementById("headerLogo");
const divAnalysisButton = document.getElementById("divAnalysisButton");
const analysisButtonCuntries = document.getElementById("AnalysisButtonCuntries");
const analysisButtonIndustries = document.getElementById("AnalysisButtonIndustries");
const analysisButtonSectors = document.getElementById("AnalysisButtonSectors");
const analysisButtonCompanies = document.getElementById("AnalysisButtonCompanies");
const fullCompositionButton = document.getElementById("FullCompositionButton");

headerLogo.addEventListener("click", () => location.replace("index.html"));
divAnalysisButton.addEventListener("click", (e) => {e.stopPropagation(); location.replace("diversificationAnalysis.html");});
analysisButtonCuntries.addEventListener("click", (e) => {e.stopPropagation(); location.replace("diversificationAnalysis.html?section=Countries");});
analysisButtonIndustries.addEventListener("click", (e) => {e.stopPropagation(); location.replace("diversificationAnalysis.html?section=Industries");});
analysisButtonSectors.addEventListener("click", (e) => {e.stopPropagation(); location.replace("diversificationAnalysis.html?section=Sectors");});
analysisButtonCompanies.addEventListener("click", (e) => {e.stopPropagation(); location.replace("diversificationAnalysis.html?section=Companies");});
fullCompositionButton.addEventListener("click", (e) => {e.stopPropagation(); location.replace("fullComposition.html");})


investmentPortfolioButton.addEventListener("click", () => 
   location.replace("investmentPortfolio.html"));