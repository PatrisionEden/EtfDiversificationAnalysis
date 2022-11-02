// let IsinToPrice = new Map();
// let IsinToTicker = new Map();

// async function LoadIsinToPrice(){
//    const response = await fetch("/getisintopricedictionary");
//    isinToPriceArr = await response.json();
//    console.log(isinToPriceArr);
//    isinToPriceArr.forEach(element => {
//       IsinToPrice.set(element.Key, element.Value);
//    });
// };

// async function LoadIsinToTicker(){
//    const response = await fetch("/getisintotickerdictionary");
//    isinToTickerArr = await response.json();
//    console.log(isinToTickerArr);
//    isinToTickerArr.forEach(element => {
//       IsinToTicker.set(element.Key, element.Value);
//    });
// };