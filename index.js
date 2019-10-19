var votes = {};
var isVoteRunning = false;

var ComfyJS = require( "comfy.js" );
ComfyJS.onCommand = ( user, command, message, flags, extra ) => {
  if( isVoteRunning && command === "vote" ) {
    votes[ user ] = message;
  }
};
ComfyJS.Init( "instafluff" );

var ComfyWeb = require( "webwebweb" );
ComfyWeb.APIs[ "/simsfactor/vote" ] = ( qs ) => {
  console.log( qs );
  votes = {};
  isVoteRunning = true;
  return {};
};
ComfyWeb.APIs[ "/simsfactor/results" ] = ( qs ) => {
  isVoteRunning = false;
  return votes;
};
ComfyWeb.Run( 8090 );
