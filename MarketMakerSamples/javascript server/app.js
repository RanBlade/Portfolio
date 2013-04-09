/*
MESSAGE FORMAT -------
 len - javascript server need to add this EVERYTIME FIRST
 functionCode - the actual code used to determine what the message is
 Destination ID - the ID of the destination for this message
 Source ID - your ID so the message reciver to knows who to send a response to if one is needed.
 Data- this is variable length... ie for SERVER_SEND_MESSAGESERVER_ID (messageserver side) all you add is 1 UINT16

example message - 
	len - variable
	functionCode - USER_SELL_SHARE
	destID - STATE_SERVER
	SourceID - serverID
	DATA - SHARE_TYPE
	       SHARE_SELL_VALUE

There will be a well defined list of Message's and they will need to be packed the same
way each time. If this is done incorrectly it will cause bugs that will not be errors(unless
said bug causes a fault ie trying to stuff a string into a UINT16)
*/

//constant variables for messege system to server.
var SERVER_REQUEST_MESSAGESERVER_ID = 10000;
var SERVER_SEND_MESSAGESERVER_ID    = 10001;
var SERVER_CLIENT_LOGIN             = 10002;
var USER_LOGIN_SUCCESS              = 10003;
var USER_LOGIN_FAIL                 = 10004;
var SERVER_USER_LOGOUT              = 10005;
var SERVER_CLIENT_FORCE_CLOSE       = 10006;
var USER_AGENT_EVENT_UPDATE_SEND_TO_ALL = 9999;
var USER_REQUEST_LOGON              = 11000;

var EVENT_SPECFIC_MESSAGE_LOW = 15000;
var EVENT_SPECFIC_MESSAGE_HIGH = 20000;

var EVENT_START         =       15001;
var EVENT_END           =       15002;
var EVENT_TRANSACTION   =       15003;
var EVENT_USER_CREATE_ORDER  =  15004;
var EVENT_USER_CANCEL_ORDER   = 15005;
var EVENT_USER_MODIFY_ORDER =   15006;
var EVENT_PLAYER_JOINS_GROUP  = 15007;
var EVENT_PLAYER_LEAVES_GROUP=  15008;
var EVENT_PLAYER_JOINS_EVENT =  15009;
var EVENT_PLAYER_QUITS_EVENT  = 15010;

var EVENT_EVENT_CREATES_GROUP = 15011;
var EVENT_EVENT_REMOVES_GROUP = 15012;
var EVENT_EVENT_UPDATES_MARKET= 15013;

var EVENT_NEW_PLAYER       =    15014;
var EVENT_PLAYER_LOGIN    =     15015;
var EVENT_REMOVE_EVENT    =     15017;

var EVENT_PLAYER_LOADED_APP =   15500;
var SERVER_UPDATE_ORDER_QTY =   15501;
var SERVER_UPDATE_ORDER_PRICE = 15502;
var UPDATE_CLIENT_INFO        = 15503;

//constants
var ORDER_TYPE_BUY = 1;
var ORDER_TYPE_SELL = 2;
//ADMIN FUNCTIONS TO BE USED BY TEAM TO MANAGE EVENTS/GROUPS/PLAYERS

var ADMIN_CREATE_EVENT    =     20001;
var ADMIN_REMOVE_EVENT    =     21000;
var ADMIN_START_EVENT     =     22000;
var ADMIN_STOP_EVENT      =     23000;

var ADMIN_ADD_COMPETITOR    =   24000;
var ADMIN_REMOVE_COMPETITOR =   24100;
var PING_USER_STILL_CONNECTED = 25000;
var PING_USER_CHALLENGE_RESP  = 25001;
//////////////////////////////////////////////////
//SERVER variables 
var serverID = 0;
var serverName = "none";
function ReplyContainer( message , userConnection)
{
	this.connection = userConnection;
	this.message = message;
}
var ReplyArray = new Array();
//SOCKET CONNECTION TO MESSAGE SERVER ///////////
var net = require('net');
var netSocketConnected = false;
var HOST = "localhost";
var PORT = 7516;
var message;

var client = net.createConnection( PORT , HOST, function(){
	console.log('Connected to: ' + HOST +':' + PORT);
	client.setNoDelay(true);
	client.setTimeout(0);
	client.setKeepAlive(true);
	netSocketConnected = true;

	var len     = 2+4;	
	message = new Buffer(8);
	message.writeUInt16LE(len,0);
	message.writeUInt16LE( SERVER_REQUEST_MESSAGESERVER_ID, 2);
	client.write( message);	
});

client.on('data',function(data){
	
	
	var dataReadPos = 0;
	
	while(dataReadPos < data.length  - 2){ // <----------------TOOK OUT THE -2 
	console.log("dataReadPos: " + dataReadPos);
	//var messageReadPos = 0;
	var messageLen = data.readUInt16LE(dataReadPos);
	//messageReadPos+=2;
	dataReadPos+=2;
	
	console.log("Data Recieved: " + data.length + " | MessageLen: " + messageLen);
	var functionCode = data.readUInt16LE(dataReadPos);
	dataReadPos+=2;
	//messageReadPos+=2;
	console.log('functionCode:'  + functionCode);
	
	if(functionCode == SERVER_SEND_MESSAGESERVER_ID)
	{
		//while(messageReadPos < messageLen){
		serverID = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		//messageReadPos+=2;
		console.log("serverID: " + serverID.toString());
		//}
	}
	if(functionCode == USER_AGENT_EVENT_UPDATE_SEND_TO_ALL)
	{
		console.log("USER_AGENT_EVENT_UPDATE_SEND_TO_ALL");
		try{		
				var t_agentID = data.readUInt16LE(dataReadPos);
				//console.log(t_agentID);
				//console.log("readPos " + readPos);
				dataReadPos += 2;
				var t_eventCount = data.readUInt16LE(dataReadPos);
				//console.log("EventCount: " + t_eventCount);
				dataReadPos += 2;
				//console.log("readPos " + readPos);
				var data1 = new Array();
				data1.push(t_agentID);
				data1.push(t_eventCount);
				for(var ii = 0; ii < t_eventCount; ii++)
				{
					var t_eventID = data.readUInt16LE(dataReadPos);
					dataReadPos+= 2;
					//console.log("readPos " + readPos);
					data1.push(t_eventID);
					var t_competitorCount = data.readUInt16LE(dataReadPos);
					dataReadPos+= 2;
					//console.log("readPos " + readPos);
					data1.push(t_competitorCount);
					//console.log("Event: " + ii);
					//console.log("CompetitorCount: " + t_competitorCount);
					for(var iii = 0; iii < t_competitorCount; iii++){
						var t_competitor = data.readUInt16LE(dataReadPos);
						dataReadPos+= 2;
						var t_competitorMLHI = data.readUInt16LE(dataReadPos);
						dataReadPos+=2;
						var t_competitorShareHI = data.readUInt16LE(dataReadPos);
						dataReadPos+=2;
						var t_competitorMarketLO = data.readUInt16LE(dataReadPos);
						dataReadPos+=2;
						var t_competitorShareLO = data.readUInt16LE(dataReadPos);
						dataReadPos+=2;
						
						//console.log("readPos " + readPos);
						data1.push(t_competitor);
						data1.push(t_competitorMLHI);
						data1.push(t_competitorShareHI);
						data1.push(t_competitorMarketLO);
						data1.push(t_competitorShareLO);
						//console.log("Competitor: " + iii);
					}
					var t_GroupCount = data.readUInt16LE(dataReadPos);
					data1.push(t_GroupCount);
					console.log('GroupCount: ' + t_GroupCount);
					dataReadPos+=2;
					for(var x = 0; x < t_GroupCount; x++) //All Event Groups
					{
						var t_groupID = data.readUInt16LE(dataReadPos);
						dataReadPos+=2;
						console.log('GroupID: ' + t_groupID);
						var t_playerCount = data.readUInt16LE(dataReadPos);
						dataReadPos+=2;
						console.log('PlayerCount: ' + t_playerCount);
						var t_competitorCount = data.readUInt16LE(dataReadPos);
						dataReadPos+=2;
						data1.push(t_groupID);
						data1.push(t_playerCount);
						data1.push(t_competitorCount);
						for(var xx = 0 ; xx < t_competitorCount; xx++)
						{
							var t_compID = data.readUInt16LE(dataReadPos);
							dataReadPos+=2;
							var t_marketHI = data.readUInt16LE(dataReadPos);
							dataReadPos+=2;
							var t_sharesHI = data.readUInt16LE(dataReadPos);
							dataReadPos+=2;
							var t_marketLO = data.readUInt16LE(dataReadPos);
							dataReadPos+=2;
							var t_sharesLO = data.readUInt16LE(dataReadPos);
							dataReadPos+=2;
							data1.push(t_compID);
							data1.push(t_marketHI);
							data1.push(t_sharesHI);
							data1.push(t_marketLO);
							data1.push(t_sharesLO);
						}
					}
					var t_EventPlayerCount = data.readUInt16LE(dataReadPos);
					dataReadPos+=2;
					var t_startTime = data.readInt32LE(dataReadPos);
					dataReadPos+= 4;
					data1.push(t_GroupCount);
					data1.push(t_EventPlayerCount);
					data1.push(t_startTime);
				}
		SendToAll('UPDATE_EVENTS' , data1);
		}
		catch(e){
		console.log('ERROR: ' + e.message);
		}
	}
	if(functionCode == EVENT_PLAYER_LOADED_APP)
	{
		console.log('RECEIVED: EVENT_PLAYER_LOADED_APP');
		try{
				var t_clientID = data.readUInt16LE(dataReadPos);
				dataReadPos+=2;
				console.log('CLIENT ID FOR MESSAGE: ' + t_clientID);
				var t_agentID = data.readUInt16LE(dataReadPos);
				console.log(t_agentID);
				//console.log("readPos " + readPos);
				dataReadPos+= 2;
				var t_eventCount = data.readUInt16LE(dataReadPos);
				//console.log("EventCount: " + t_eventCount);
				dataReadPos+= 2;		
				for(var ii = 0; ii < t_eventCount; ii++)
				{
					//console.log("readPos " + readPos);
					var data1 = new Array();
					data1.push(t_agentID);
					data1.push(t_eventCount);
					var t_eventID = data.readUInt16LE(dataReadPos);
					dataReadPos+= 2;
					//console.log("readPos " + readPos);
					data1.push(t_eventID);
					var t_competitorCount = data.readUInt16LE(dataReadPos);
					dataReadPos+= 2;
					//console.log("readPos " + readPos);
					data1.push(t_competitorCount);
					//console.log("Event: " + ii);
					//console.log("CompetitorCount: " + t_competitorCount);
					for(var iii = 0; iii < t_competitorCount; iii++){
						var t_competitor = data.readUInt16LE(dataReadPos);
						dataReadPos+= 2;
						var t_competitorMLHI = data.readUInt16LE(dataReadPos);
						dataReadPos+=2;
						var t_competitorShareHI = data.readUInt16LE(dataReadPos);
						dataReadPos+=2;
						var t_competitorMarketLO = data.readUInt16LE(dataReadPos);
						dataReadPos+=2;
						var t_competitorShareLO = data.readUInt16LE(dataReadPos);
						dataReadPos+=2;
						
						//console.log("readPos " + readPos);
						data1.push(t_competitor);
						data1.push(t_competitorMLHI);
						data1.push(t_competitorShareHI);
						data1.push(t_competitorMarketLO);
						data1.push(t_competitorShareLO);
						//console.log("Competitor: " + iii);
					}
					var t_GroupCount = data.readUInt16LE(dataReadPos);
					data1.push(t_GroupCount);
					console.log('GroupCount: ' + t_GroupCount);
					dataReadPos+=2;
					for(var x = 0; x < t_GroupCount; x++) //All Event Groups
					{
						var t_groupID = data.readUInt16LE(dataReadPos);
						dataReadPos+=2;
						console.log('GroupID: ' + t_groupID);
						var t_playerCount = data.readUInt16LE(dataReadPos);
						dataReadPos+=2;
						console.log('PlayerCount: ' + t_playerCount);
						var t_competitorCount1 = data.readUInt16LE(dataReadPos);
						dataReadPos+=2;
						data1.push(t_groupID);
						data1.push(t_playerCount);
						data1.push(t_competitorCount1);
						for(var xx = 0 ; xx < t_competitorCount1; xx++)
						{
							var t_compID = data.readUInt16LE(dataReadPos);
							dataReadPos+=2;
							var t_marketHI = data.readUInt16LE(dataReadPos);
							dataReadPos+=2;
							var t_sharesHI = data.readUInt16LE(dataReadPos);
							dataReadPos+=2;
							var t_marketLO = data.readUInt16LE(dataReadPos);
							dataReadPos+=2;
							var t_sharesLO = data.readUInt16LE(dataReadPos);
							dataReadPos+=2;
							data1.push(t_compID);
							data1.push(t_marketHI);
							data1.push(t_sharesHI);
							data1.push(t_marketLO);
							data1.push(t_sharesLO);
						}
					}
					var t_EventPlayerCount = data.readUInt16LE(dataReadPos);
					dataReadPos+=2;
					var t_startTime = data.readInt32LE(dataReadPos);
					dataReadPos+= 4;
					data1.push(t_GroupCount);
					data1.push(t_EventPlayerCount);
					data1.push(t_startTime);
					SendToClient('EVENT_PLAYER_LOADED_APP_ACK' , t_clientID , data1);
				}
				}
		catch(e){
			console.log('ERROR: ' + e.message);
		}			
		//for(var i = ReplyArray.length -1; i >= 0; i--)
		//{
			//if(ReplyArray[i].message == EVENT_PLAYER_LOADED_APP)
			//{
				//console.log('Server replied with event info for client');
				//ReplyArray[i].sock.emit('EVENT_PLAYER_LOADED_APP_ACK' , data1);				
				//ReplyArray.splice(i , 1);
			//}
		//}
		
	}
	if(functionCode == EVENT_REMOVE_EVENT)
	{
		console.log('Send Remove_EVENT flag to all users');
		var t_eventID = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var data1 = new Array();
		data1.push(t_eventID);
		SendToAll('EVENT_REMOVED' , data1);
	}
	if(functionCode == USER_LOGIN_SUCCESS)
	{
try{
		var clientID = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var PlayerScore = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var PlayerBalance = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var myEventsCount = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var data1 = new Array();
		data1.push(clientID);
		data1.push(PlayerScore);
		data1.push(PlayerBalance);
		data1.push(myEventsCount);
		if(myEventsCount)
		{//> //0 && myEventsCount < messageLen - 
		for(var i = 0; i < myEventsCount; i++)
		{
			var eventID = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			var grpID = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			var eventBalance = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			data1.push(eventID);
			data1.push(grpID);
			data1.push(eventBalance);
		}
		}
		var myShareCounter = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		data1.push(myShareCounter);
		console.log("ShareCount:" + myShareCounter);
		if(myShareCounter > 0)
		{
		for(var ii = 0; ii < myShareCounter; ++ii)
		{
			
			var competitorID = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			console.log("CompetitorID:" + competitorID);
			var shareQty = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			console.log("shareQty:" + shareQty);
			var eventID = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			console.log("eventID: " + eventID);
			var grpID = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			console.log("grpID:"  + grpID);
	
			data1.push(competitorID);
			data1.push(shareQty);
			data1.push(eventID);
			data1.push(grpID);
		}
		}
		var myOrderCount = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		data1.push(myOrderCount);
		if(myOrderCount > 0)
		{
		console.log("LOADING ORDER===================");
		console.log("OrderCount: " + myOrderCount);
		for(var x = 0; x < myOrderCount; ++x)
		{
			var orderID= data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			var eventID = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			var grpID = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			var ownerID= data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			var competitorID= data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			var typeofOrder = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			var qty = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			var price = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			var time = data.readInt32LE(dataReadPos);
			dataReadPos+=4;
			data1.push(orderID);
			console.log("OID:" + orderID);
			data1.push(eventID);
			console.log("EID: " + eventID);
			data1.push(grpID);
			console.log("GID: " + grpID);
			data1.push(ownerID);
			console.log("OWNID: " + ownerID);
			data1.push(competitorID);
			console.log("COMPID: " + competitorID);
			data1.push(typeofOrder);
			console.log("TYPE: " + typeofOrder);
			data1.push(qty);
			console.log("QTY: " + qty);
			data1.push(price);
			console.log("PRICE: " + price);
			data1.push(time);
			console.log("TIME: " + time);
			console.log("================================");
		}
		}
		SendToClient('USER_LOGIN_SUCCESS' , clientID , data1);
		}
		catch(e){
			console.log('ERROR: ' + e.message);
			SendToClient('USER_LOGIN_SUCCESS' , clientID , data1);
			console.log(data1);
		}
	}
	if(functionCode == UPDATE_CLIENT_INFO)
	{
	try{
		var clientID = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var PlayerScore = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var PlayerBalance = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var myEventsCount = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var data1 = new Array();
		data1.push(clientID);
		data1.push(PlayerScore);
		data1.push(PlayerBalance);
		data1.push(myEventsCount);
		if(myEventsCount > 0)
		{
		for(var i = 0; i < myEventsCount; i++)
		{
			var eventID = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			var grpID = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			var eventBalance = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			data1.push(eventID);
			data1.push(grpID);
			data1.push(eventBalance);
		}
		}
		var myShareCounter = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		data1.push(myShareCounter);
		if(myShareCounter > 0)
		{
		console.log("ShareCount:" + myShareCounter);
		for(var ii = 0; ii < myShareCounter; ++ii)
		{
			
			var competitorID = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			//console.log("CompetitorID:" + competitorID);
			var shareQty = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			//console.log("shareQty:" + shareQty);
			var eventID = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			//console.log("eventID: " + eventID);
			var grpID = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			//console.log("grpID:"  + grpID);
	
			data1.push(competitorID);
			data1.push(shareQty);
			data1.push(eventID);
			data1.push(grpID);
		}
		}
		var myOrderCount = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		data1.push(myOrderCount);
		if(myOrderCount > 0)
		{
		//console.log("LOADING ORDER===================");
		console.log("OrderCount: " + myOrderCount);
		for(var x = 0; x < myOrderCount; ++x)
		{
			var orderID= data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			var eventID = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			var grpID = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			var ownerID= data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			var competitorID= data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			var typeofOrder = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			var qty = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			var price = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			var time = data.readInt32LE(dataReadPos);
			dataReadPos+=4;
			data1.push(orderID);
			//console.log("OID:" + orderID);
			data1.push(eventID);
			//console.log("EID: " + eventID);
			data1.push(grpID);
			//console.log("GID: " + grpID);
			data1.push(ownerID);
			//console.log("OWNID: " + ownerID);
			data1.push(competitorID);
			//console.log("COMPID: " + competitorID);
			data1.push(typeofOrder);
			//console.log("TYPE: " + typeofOrder);
			data1.push(qty);
			//console.log("QTY: " + qty);
			data1.push(price);
			//console.log("PRICE: " + price);
			data1.push(time);
			//console.log("TIME: " + time);
			//console.log("================================");
		}
		}
		SendToClient('UPDATE_CLIENT_DATA' , clientID , data1);
		}
		catch(e){
			console.log('ERROR: ' + e.message);
			console.log('UPDATE_CLIENT_DATA');
			return;
		}
	}
	
	if(functionCode == USER_LOGIN_FAIL)
	{
		var clientID = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var data1 = new Array();
		data1.push(clientID);
		SendToClient('USER_LOGIN_FAIL' , clientID ,  data1);
	}
	if(functionCode == EVENT_PLAYER_JOINS_EVENT)
	{
	try{
		var clientID = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var eventID = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var grpID = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var eventBalance = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var shareCount = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var data1 = new Array();
		data1.push(clientID);
		data1.push(eventID);
		data1.push(grpID);
		data1.push(shareCount);
		for(var i = 0; i < shareCount; i++)
		{
			var competitorID = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			var shareQty = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			data1.push(competitorID);
			data1.push(shareQty);
		}
		SendToClient('CLIENT_JOINED_EVENT' , clientID , data1);
		}
		catch(e){
		console.log('ERROR: ' + e.message);
		}
	}
	if(functionCode == EVENT_USER_CREATE_ORDER)
	{
		var eventID = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var clientID = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var orderID = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var orderType = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var competitorID = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var qty = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var price = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var time = data.readInt32LE(dataReadPos);
		dataReadPos+=2;
		
		var data1 = new Array();
		data1.push(eventID);
		data1.push(clientID);
		data1.push(orderID);
		data1.push(orderType);
		data1.push(competitorID);
		data1.push(qty);
		data1.push(price);
		data1.push(time);
		SendToClient('ADD_ORDER' , clientID , data1);
	}
	if(functionCode == SERVER_UPDATE_ORDER_QTY)
	{
		var eventID = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var orderID = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var clientID = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var qty = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		
		var data1 = new Array();
		data1.push(eventID);
		data1.push(orderID);
		data1.push(clientID);
		data1.push(qty);
		
		SendToClient('UPDATE_ORDER_QTY' , clientID , data1);
	}
	if(functionCode == SERVER_UPDATE_ORDER_PRICE)
	{
		var eventID = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var orderID = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var clientID = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var price = data.readUInt16LE(dataReadPos);
		
		var data1 = new Array();
		data1.push(eventID);
		data1.push(orderID);
		data1.push(clientID);
		data1.push(price);
		
		SendToClient('UPDATE_ORDER_PRICE' , clientID , data1);
		}
	}
	if(functionCode == EVENT_USER_CANCEL_ORDER)
	{
		var eventID = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var clientID = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var orderID = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		var data1 = new Array();
		data1.push(eventID);
		data1.push(clientID);
		data1.push(orderID);
		/*data1.push(orderType);
		if(orderType == ORDER_TYPE_BUY)
		{
			var playerEventBalance = data.readUInt16LE(dataReadPos)
			dataReadPos+=2;
			data1.push(playerEventBalance);
		}
		if(orderType == ORDER_TYPE_SELL)
		{
			var compID = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			var qty = data.readUInt16LE(dataReadPos);
			dataReadPos+=2;
			
			data1.push(compID);
			data1.push(qty);
		}*/

		SendToClient('REMOVE_ORDER' , clientID , data1);
	}
	if(functionCode == PING_USER_STILL_CONNECTED)
	{
		var clientID = data.readUInt16LE(dataReadPos);
		dataReadPos+=2;
		console.log("Pinging clientID:" +clientID);
		var data1 = new Array();
		data1.push(clientID);
		
		SendToClient('PING' , clientID , data1);
	}
});

//END SOCKET CONNECTION TO MESSAGE SERVER////////////////////////
//HELPER FUNCTIONS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
function SendToAll(functionCode , data)
{
	for(var i = 0; i < connectedWebClients.length; i++)
		{
			
			connectedWebClients[i].sock.emit(functionCode , data);
			console.log('SENDING TO ' + connectedWebClients[i].id);
		}
}
function SendToClient(functionCode , clientID , data)
{
	for(var i =0;i < connectedWebClients.length; i++)
	{
		if(connectedWebClients[i].id == clientID)
		{
			connectedWebClients[i].sock.emit(functionCode , data);
			return;
		}
	}
}

//-!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//socket.io variables----------------------------------------------
var connectedWebClients = new Array();
var connWCIndex = 0;
var m_seedID = 0;

//pseduo class for connections.
function Connection(type){
	this.type = type;
	this.sock = 0;
	this.id   = 0;
	this.ProcessConnection = function(data){
		sock.on('test' , function(data){
		console.log(data);});
	};
}
function GetConnectionID(socket)
{
	for(var i in connectedWebClients)
	{
		if(connectedWebClients[i].sock == socket)
		{
			return connectedWebClients[i].id;
		}
	}
}
function GetConnection(t_connectionID)
{
	for(var i in connectedWebClients)
	{
		if(connectedWebClients[i].id == t_connectionID)
		{
			return connectedWebClients[i];
		}
		else{
			console.log('No client at ID: ' + t_connectionID);
			return 0;
		}
	}
}
function HandleConnection(socket){
	var newConnBool = true;
	//for(var i = 0; i < connectedWebClients.length; i++)
	//{
		//if(socket == connectedWebClients[i].sock){
		//	newConnBool = false;
		//	break;
		//}
	//}
	if(newConnBool == true){
		m_seedID++;
		var newConn = new Connection("User");
		newConn.sock = socket; 
		newConn.id   = m_seedID;
		connectedWebClients.push(newConn);
		var data = [newConn.id];
		socket.emit('SERVER_ASSIGNED_CLIENT_ID_FOR_SESSION' , data);
					
	}
	socket.on('CLIENT_REQUEST_UPDATE', function(data){
		
		//var var1 = parseInt(data[0]);
		//var var2 = parseInt(data[1]);
		//var var3 = parseInt(data[2]);
		//var var4 = parseInt(data[3]);
		
		var len     = 2 + 2 + 2 + 2 + 2 + 2;
		message = new Buffer(len + 2);
		message.writeUInt16LE(len , 0);
		message.writeUInt16LE(EVENT_PLAYER_LOADED_APP , 2);
		message.writeUInt16LE(data[0] , 4);
		message.writeUInt16LE(data[1] , 6);
		message.writeUInt16LE(data[2] , 8);
		console.log("ClientID:" + data[2]);
		message.writeUInt16LE(data[3] , 10);
		client.write(message);
		var t_conn = GetConnection(data[2]);
		var replyRequest = new ReplyContainer(EVENT_PLAYER_LOADED_APP , t_conn);
		ReplyArray.push(replyRequest);	
	});
	socket.on('USER_REQUEST_LOGIN', function(data){
		console.log("Recivived socket.on 'USER_REQUEST_LOGIN' ");
		var userName = data[0];
		var clientID = parseInt(data[1]);
		var password = data[2];
		var pingchallenge = parseInt(data[3]);
		var strlenUname = userName.length;
		var strlenPwrd  =  password.length;
		var len = 2 + 2 + 2 + 2 + 2 + 2 + strlenUname + strlenPwrd;
		console.log("ClientID: " + clientID + " | PingChallenge: " + pingchallenge);
		message = new Buffer(len + 2);
		message.writeUInt16LE(len , 0);
		message.writeUInt16LE(SERVER_CLIENT_LOGIN , 2);
		message.writeUInt16LE(clientID, 4);
		message.writeUInt16LE(pingchallenge , 6);
		message.writeUInt16LE(strlenUname , 8);
		message.write(userName , 10);
		message.writeUInt16LE(strlenPwrd , (10 + strlenUname));
		message.write(password , (12 + strlenUname));
		client.write(message);
		
		
		
	});
	socket.on('USER_REQUEST_LOGOUT' , function(data){
		//var clientusername = data[0];
		var clientID = parseInt(data[1]);
		//var strLen = clientusername.length;
		var len = 2 + 2 + 2 + 2;//+ strLen;
		message = new Buffer(len + 2);
		message.writeUInt16LE(len , 0);
		message.writeUInt16LE(SERVER_USER_LOGOUT, 2);
		message.writeUInt16LE(clientID , 4);
		//message.writeUInt16LE(strLen, 6);
		//message.write(clientusername , 8);
		client.write(message);
	});
	socket.on('PONG' , function(data)
	{
		var funcCode = PING_USER_CHALLENGE_RESP;
		var clientID = parseInt(data[0]);
		var challenge = parseInt(data[1]);
		
		var len = 2 + 2 + 2 + 2;
		message = new Buffer(len + 2);
		
		message.writeUInt16LE(len , 0);
		message.writeUInt16LE(funcCode , 2);
		message.writeUInt16LE(clientID , 4);
		message.writeUInt16LE(challenge , 6);
		
		client.write(message);
	});
	socket.on('USER_REQUEST_JOIN_EVENT' , function(data)
	{
		var functionCode = EVENT_PLAYER_JOINS_EVENT;
		var agentID = parseInt(data[0]);
		var eventID = parseInt(data[1]);
		var groupID = parseInt(data[2]);
		var clientID = parseInt(data[3]);
		var len = 2 + 2 + 2 + 2 + 2 + 2;
		message = new Buffer(len + 2);
		message.writeUInt16LE(len , 0);
		message.writeUInt16LE(functionCode, 2);
		message.writeUInt16LE(agentID , 4);
		message.writeUInt16LE(eventID , 6);
		message.writeUInt16LE(groupID , 8);
		message.writeUInt16LE(clientID , 10);
		
		client.write(message);
	});
	socket.on('USER_CREATE_BUY_ORDER', function(data){
		var functionCode = EVENT_USER_CREATE_ORDER;
		var agentID = parseInt(data[0]);
		var eventID = parseInt(data[1]);
		var grpID = parseInt(data[2]);
		var clientID = parseInt(data[3]);
		var orderType = 1; //1 == buy
		var competitorID = parseInt(data[4]);
		var shareQty = parseInt(data[5]);
		var pricePerShare = parseInt(data[6]);
		var len = 2 + 2 + 2 + 2 + 2 + 2 + 2 + 2 + 2 + 2;
		
		message = new Buffer(len + 2);
		message.writeUInt16LE(len, 0);
		message.writeUInt16LE(functionCode , 2);
		
		message.writeUInt16LE(agentID , 4);
		message.writeUInt16LE(eventID , 6);
		message.writeUInt16LE(grpID , 8);
		message.writeUInt16LE(clientID , 10);
		message.writeUInt16LE(orderType , 12);
		message.writeUInt16LE(competitorID , 14);
		message.writeUInt16LE(shareQty , 16);
		message.writeUInt16LE(pricePerShare , 18);
		
		client.write(message);

	});
	socket.on('USER_CREATE_SELL_ORDER' , function(data)
	{
		var functionCode = EVENT_USER_CREATE_ORDER;
		var agentID = parseInt(data[0]);
		var eventID = parseInt(data[1]);
		var grpID = parseInt(data[2]);
		var clientID = parseInt(data[3]);
		console.log("AgentID: " + agentID);
		console.log("EVENTID: " + eventID);
		console.log("GRPID: " + grpID);
		var orderType = 2; //2 == buy
		var competitorID = parseInt(data[4]);
		var shareQty = parseInt(data[5]);
		var pricePerShare = parseInt(data[6]);
		var len = 2 + 2 + 2 + 2 + 2 + 2 + 2 + 2 + 2 + 2;
		
		message = new Buffer(len + 2);
		message.writeUInt16LE(len, 0);
		message.writeUInt16LE(functionCode , 2);
		message.writeUInt16LE(agentID , 4);
		message.writeUInt16LE(eventID , 6);
		message.writeUInt16LE(grpID , 8);
		message.writeUInt16LE(clientID , 10);
		message.writeUInt16LE(orderType , 12);
		message.writeUInt16LE(competitorID , 14);
		message.writeUInt16LE(shareQty , 16);
		message.writeUInt16LE(pricePerShare , 18);
		
		client.write(message);
	});
	socket.on('USER_MODIFY_ORDER' , function(data)
	{
		var functionCode = EVENT_USER_MODIFY_ORDER;
		var agentID = parseInt(data[0]);
		var eventID = parseInt(data[1]);
		var grpID = parseInt(data[2]);
		var clientID = parseInt(data[3]);
		var orderID = parseInt(data[4]);
		var actionOnOrder = parseInt(data[5]);
		var shareQty = parseInt(data[6]);
		var len = 2 + 2 + 2 + 2 + 2 + 2 + 2 + 2 + 2;
		
		message = new Buffer(len + 2);
		message.writeUInt16LE(len, 0);
		message.writeUInt16LE(functionCode , 2);
		message.writeUInt16LE(agentID , 4);
		message.writeUInt16LE(eventID , 6);
		message.writeUInt16LE(grpID , 8);
		message.writeUInt16LE(clientID , 10);
		message.writeUInt16LE(orderID , 12);
		message.writeInt16LE(actionOnOrder , 14);
		message.writeUInt16LE(shareQty , 16);
		
		client.write(message);
	});
	socket.on('USER_REMOVE_ORDER' , function(data)
	{
		var functionCode = EVENT_USER_CANCEL_ORDER;
		var agentID = parseInt(data[0]);
		var eventID = parseInt(data[1]);
		var grpID = parseInt(data[2]);
		var clientID = parseInt(data[3]);
		var orderID = parseInt(data[4]);
		var len = 2 + 2 + 2 + 2 + 2 + 2 + 2;
		
		message = new Buffer(len + 2);
		message.writeUInt16LE(len, 0);
		message.writeUInt16LE(functionCode , 2);
		message.writeUInt16LE(agentID , 4);
		message.writeUInt16LE(eventID , 6);
		message.writeUInt16LE(grpID , 8);
		message.writeUInt16LE(clientID , 10);
		message.writeUInt16LE(orderID , 12);
		
		client.write(message);
	});
	socket.on('ADMIN_CREATE_EVENT' , function(data){
		console.log("Got a request for admin control");
		console.log(data);

		var messageName = parseInt(data[0]);
		var code = parseInt(data[1]);
		var variable1 = data[2];
		var variable2 = parseInt(data[3]);
		var variable3 = parseInt(data[4]);
		var variable4 = parseInt(data[5]);
		var variable5 = parseInt(data[6]);
		var variable6 = data[7];
		var variable7 = data[8];
		var variable8 = data[9];
		var buffer = new Buffer(100);
		var strLen = variable1.length;
		var len = 2 + 2 + 2 + 2 + 2 + 2 + 4 + strLen;
		console.log(len);
		message = new Buffer(len + 2);
			var messagepos = 0;
			message.writeUInt16LE(len , 0);
			message.writeUInt16LE(messageName , 2);
			message.writeUInt16LE(code , 4);
			message.writeUInt16LE(variable2 , 6);
			message.writeUInt16LE(variable3 , 8);
			message.writeUInt16LE(variable4 , 10);
			message.writeInt32LE(variable5 , 12);
			message.writeUInt16LE(strLen , 16);
			message.write(variable1 , 18);

		console.log(message.length);
		client.write(message);
	});
	socket.on('ADMIN_START_EVENT' , function(data){
		console.log("Got a request for admin control");
		console.log(data);

		var messageName = parseInt(data[0]);
		var code = parseInt(data[1]);
		var variable1 = parseInt(data[2]);
		//var variable2 = parseInt(data[3]);
		//var variable3 = data[4];
		//var variable4 = data[5];
		//var variable5 = data[6];
		//var variable6 = data[7];
		//var variable7 = data[8];
		//var variable8 = data[9];
		var len = 2 + 2 + 2 + 4;
		message = new Buffer(len + 2);
		
		message.writeUInt16LE(len , 0);
		message.writeUInt16LE(messageName , 2);
		message.writeUInt16LE( code , 4);
		message.writeUInt32LE( variable1 , 6);
		
		client.write(message);
	});
	socket.on('ADMIN_REMOVE_EVENT' , function(data){
		var messageName = parseInt(data[0]);
		var code = parseInt(data[1]);
		var variable1 = parseInt(data[2]);
		var len = 2 + 2 + 2 + 2;
		message = new Buffer(len + 2);
		message.writeUInt16LE(len , 0);
		message.writeUInt16LE(messageName , 2);
		message.writeUInt16LE(code , 4);
		message.writeUInt16LE(variable1 , 6);
		client.write(message);
	});
	socket.on('disconnect' , function(data){
	
		var clientID = GetConnectionID(socket);
		var len = 2 + 2 + 2;
		message = new Buffer(len + 2);
		message.writeUInt16LE(len , 0);
		message.writeUInt16LE(SERVER_CLIENT_FORCE_CLOSE , 2);
		message.writeUInt16LE(clientID , 4);
		client.write(message);
		
		for(var i = 0; i < connectedWebClients.length; i++)
		{
			if(socket == connectedWebClients[i].sock){
				console.log("Removing Connection ID:" + connectedWebClients[i].id);
				connectedWebClients.splice(i , 1);
				break;
			}
		}
	});
	
}

//-----------------------------------------------------------------
//SOCKET.IO SERVER TO SERVE WEBPAGE CLIENT///////////////////////
var app = require('http').createServer(handler)
  , io = require('socket.io').listen(app)
  , fs = require('fs')

app.listen(7517);

function handler (req, res) {
  fs.readFile(__dirname + '/index.html',
  function (err, data) {
    if (err) {
      res.writeHead(500);
      return res.end('Error loading index.html');
    }

    res.writeHead(200);
    res.end(data);
  }); 
}

io.sockets.on('connection', function(socket) {
	HandleConnection(socket);


});

function Encode_USER_LOGON(funcCode)
{
	
}

//END SOCKET.IO SERVER TO SERVE WEBPAGE CLIENT/////////////////////
