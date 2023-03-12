@ECHO OFF
"..\JavascriptEncoder\bin\Debug\net7.0\JavascriptEncoder.exe" yu.js
COPY ".\out\yu.js" ".\bin\Debug\net7.0\yu.js"
ECHO "Encoded yu.js"