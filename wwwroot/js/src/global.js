// Wire up signalr and js intervals -> preparePage() is hooked from navigation script on page change

const jsInterval = {}
const signalrConnections = {}

/* eslint-disable no-unused-vars */
function runJsInterval(script, str, delay) {
  if (!jsInterval[str]) {
    // console.log("Not in array")
    jsInterval[str] = setInterval((el) => el.script, delay)
  }
}

function clearJsInterval() {
  for (const key in jsInterval) {
    if (Object.prototype.hasOwnProperty.call(jsInterval, key)) {
      clearInterval(jsInterval[key])
      jsInterval[key] = null
      delete jsInterval[key]
    }
  }
}

/* eslint-disable no-unused-vars */
function addSignalrClient(name, connection) {
  if (!signalrConnections[name]) {
    // console.log("Not in array")
    signalrConnections[name] = connection
  }
}

function cleanSignalr() {
  for (const key in signalrConnections) {
    if (Object.prototype.hasOwnProperty.call(signalrConnections, key)) {
      // console.log("Disconnect")
      // console.log(signalrConnections[key])
      signalrConnections[key].stop()
      delete signalrConnections[key]
    }
  }
}

function preparePage() {
  clearJsInterval()
  cleanSignalr()
}
