//console.log('starting network.js');
var socket = io.connect('http://50.28.103.22:7517');

/* ALL Message types under this section that have _ACK are server response to webapp
	   USER_REQUEST_UPCOMING_EVENTS
	   USER_REQUEST_UPCOMING_EVENTS_ACK
	   USER_REQUEST_MY_EVENTS
	   USER_REQUEST_MY_EVENTS_ACK
	   USER_REQUEST_LOGIN
           USER_REQUEST_LOGIN_ACK -- On succeful login this message sends USER data (ie balance name) and Event Data
	   USER_REQUEST_LOGOUT
	   USER_REQUEST_LOGOUT_ACK
           USER_REQUEST_JOIN_EVENT
           USER_REQUEST_JOIN_EVENT_ACK
	   USER_REQUEST_ORDER
	   USER_REQUEST_ORDER_ACK
	   USER_REQUEST_UPDATE_DATA --PlayerINFO and EventINFO 
 	   USER_REQUEST_UPDATE_DATA_ACK
 All Message types under this section that have _ACK are webapp responses to server */
 //CONSTANTS
 var REDUCE_ORDER_PRICE = -1;
 var INCREASE_ORDER_PRICE = 1;
 var REDUCE_ORDER_QTY = -2;
 var INCREASE_ORDER_QTY = 2;
 
 var ORDER_TYPE_BUY = 1;
 var ORDER_TYPE_SELL = 2;
 
var ORDER_STATUS_OPEN = 15;
var ORDER_STATUS_WON  = 16;
var ORDER_STATUS_SOLD = 17;
var ORDER_STATUS_CANCELED = 18;
 /////////////////////////////////
function Player()
{ 
	this.PlayerID;
	this.PlayerName;
	this.PlayerScore;
	this.PlayerShares = new Array(); // this will be an array of PlayerShares ... 
}
function Shares()
{
	this.quanity;
	this.competitorID;
	this.eventID;
	this.grpID;
}
function Competitor()
{
	this.CompetitorID;
	this.CompetitorName; //with your table may not need this...
	this.MakertLineHI;
	this.sharesAtHI;
	this.MarketLineLOW;
	this.sharesAtLOW;
	this.price;
}
function LeaderBoardEntry()
{
	this.playerID;
	this.playerScore;
}
function Group()
{
	this.GroupID;
	//this.GroupPlayers = new Array();
	this.PlayerCount;
	this.Competitors = new Array();
	this.LeaderBoard = new Array();
}
function Order()
{
	this.OrderID;
	this.OrderType; //BUY OR SELL
	this.OwnerID;
	this.CompetitorID;
	this.size;
	this.price;
	this.status;
	this.time;
}
function Event()
{
	this.EventID;
	this.AgentID; //this is only needed for the server to know which Agent is handling this event. It is not needed for display.
	this.Competitors = new Array();
	this.sportleague;
	this.orders = new Array();
	this.groups = new Array();
	this.start;
	this.groupCount;
	this.eventPlayerCount;
	
	this.GetGroup = function(groupID)
	{
		console.log("Looking for Group in Event:" + this.EventID);
		for(var g in this.groups)
		{
			console.log("GroupID: " + this.groups[g].GroupID);
			console.log("Looking for Group: " + groupID);
			if(this.groups[g].GroupID == groupID)
			{
				var group = this.groups[g];
				console.log("Found Group");
				return group;
			}
		}
	}
	this.GetOrder = function(orderID)
	{
		for(var o in this.orders)
		{
			if(this.orders[o].OrderID == orderID)
			{
				var order = this.orders[o];
				console.log("Found Order");
				return order;
			}
			
		}
		return undefined;
	}
	this.RemoveOrder = function(orderID)
	{
		for(var o in this.orders)
		{
			if(this.orders[o].OrderID == orderID)
			{
				this.orders.splice(o , 1);
				_main.forceClientRefresh();
				return true;
			}
		}
		return false;
	}
	this.AddOrder = function(ORDER)
	{
		this.orders.push(ORDER);
		_main.forceClientRefresh();
	}
}
function MyEvents()
{
	this.eventID;
	this.grpID;
	this.eventBalance;
}
function DATA()
{
	this.UserID;
	this.Username;
	this.UserBalance;
	this.UserScore;
	this.pingChallenge = 0;
	this.shares = new Array();
	this.MyEvents = new Array();
	this.events = new Array();
	
	//member functions
	this.GetMyEvent = function(eventID){
		for(e in this.MyEvents)
		{
			if(this.MyEvents[e].eventID == eventID){
				return this.MyEvents[e];
			}
		}
		return undefined;
	}
	this.GetEvent = function(eventID){
		for(var e in this.events)
		{
			if(this.events[e].EventID == eventID){
				var event = this.events[e];
				console.log("found event");
				return event;
			}
		}
		return undefined;
	}
	this.GetShare = function(COMPID)
	{
		for(var i = 0; i < this.shares.length; i++)
		{
			if(this.shares[i].competitorID == COMPID)
			{
				return this.shares[i]
			}
		}
	}
	this.GetGroup = function(GROUPID)
	{
		for(var e in this.events)
		{
			for(var g in this.events[e].groups)
			{
				if(this.events[e].groups[g].GroupID == GROUPID)
				{
					return this.events[e].groups[g];
				}
			}
		}
	}
	this.RemoveEvent = function(eventID){
		for(var e in this.events)
		{
			if(this.events[e].EventID == eventID)
			{
				this.events.splice(e , 1);
			}
		}
		return undefined;
	}
	this.UpdateEvent = function(t_event)
	{
		var savedEvent = this.GetEvent(t_event.EventID);
		if(savedEvent == undefined)
		{
			return false;
		}
		savedEvent.AgentID = t_event.AgentID;
		savedEvent.Competitors = t_event.Competitors;
		savedEvent.sportleague = t_event.sportleague;
		savedEvent.groups = t_event.groups;
		savedEvent.start = t_event.start;
		savedEvent.groupCount = t_event.groupCount;
		savedEvent.eventPlayerCount = t_event.eventPlayerCount;
		return true;
	}
	this.ClearOrders = function()
	{
		for(var e in this.events)
		{
			this.events[e].orders = [];
		}
	}
	this.GetCompetitorHigestBidShareCount= function(EVENTID , GRPID , COMPETITORID)
	{
		for(var e in this.events)
		{
			if(this.events[e].EventID == EVENTID)
			{
				for(var g in this.events[e].groups)
				{
					if(this.events[e].groups[g].GroupID == GRPID)
					{
						for(var c in this.events[e].groups[g].Competitors)
						{
							if(this.events[e].groups[g].Competitors[c].CompetitorID == COMPETITORID)
							{
								return this.events[e].groups[g].Competitors[c].sharesAtHI;
							}
						}
					}
				}
			}
		}
	}
	this.GetCompetitorLowestBidShareCount= function(EVENTID , GRPID , COMPETITORID)
	{
		for(var e in this.events)
		{
			if(this.events[e].EventID == EVENTID)
			{
				for(var g in this.events[e].groups)
				{
					if(this.events[e].groups[g].GroupID == GRPID)
					{
						for(var c in this.events[e].groups[g].Competitors)
						{
							if(this.events[e].groups[g].Competitors[c].CompetitorID == COMPETITORID)
							{
								return this.events[e].groups[g].Competitors[c].sharesAtLOW;
							}
						}
					}
				}
			}
		}
	}
	this.GetMyShareQty = function(COMPETITORID)
	{
		for(var s in this.shares)
		{
			if(this.shares[s].competitorID == COMPETITORID)
			{
				return this.shares[s].quanity;
			}
		}
	}
	this.CheckMyEventsForEventID = function(EVENTID)
	{
		for(var i in this.MyEvents)
		{
			if(this.MyEvents[i].eventID == EVENTID)
			{
				return true;
			}
		}
		return false;
	}

	this.GetOrder = function(ORDERID)
	{
		for(var e in this.events)
		{
			for(var o in this.events[e].orders)
			{
				if(this.events[e].orders[o].OrderID == ORDERID && ClientDATA.UserID == this.events[e].orders[o].OwnerID)
				{
					return this.events[e].orders[o];
				}
			}
		}
		return undefined;
	}
	this.GetEventForOrder = function(ORDERID)
	{
		for(var e = 0; e < this.events.length; e++)
		{
			console.log(this.events[e]);
			console.log("Checking event at ID: " + e);
			var orders = this.events[e].orders;
			console.log(orders);
			console.log("Cshowed var orders");
			var event = this.events[e]
			for(var o = 0; o < orders.length; o++)
			{
				if(orders[o].OrderID == ORDERID )//&& ClientDATA.UserID == orders[o].OwnerID)
				{
					console.log("FOUND EVENT");
					return event;
				}
			}
		}
		return undefined;
	}
	this.GetOrderAndEventID = function(ORDERID)
	{
		for(var e in this.events)
		{
			for(var o in this.events[e].orders)
			{
				if(this.events[e].orders[o].OrderID == ORDERID && ClientDATA.UserID == this.events[e].orders[o].OwnerID)
				{
					return [this.events[e].eventID , this.events[e].orders[o]];
				}
			}
		}
		return undefined;
	}
	
}

var ClientDATA = new DATA();

var MyPlayer = new Player();
MyPlayer.PlayerID = ClientDATA.UserID;
MyPlayer.PlayerName = ClientDATA.Username;
MyPlayer.PlayerScore = ClientDATA.UserScore;

var OtherPlayer = new Player();
OtherPlayer.PlayerID = 75;
OtherPlayer.PlayerName = 'Eric';
OtherPlayer.PlayerScore = 0;

function UserLogin(USERNAME , PASSWORD)
{
	ClientDATA.pingChallenge = Math.floor(Math.random()* 15001);
	var data = [USERNAME , ClientDATA.UserID , PASSWORD , ClientDATA.pingChallenge];
	ClientDATA.Username = USERNAME;
	
	socket.emit('USER_REQUEST_LOGIN' , data);
}
function UserLogout()
{
	var data = [ClientDATA.Username , ClientDATA.UserID];
	socket.emit('USER_REQUEST_LOGOUT' , data);
	console.log("USER_REQUEST_LOGOUT");
	ClientDATA.MyEvents = [];
	ClientDATA.Username = undefined;
	ClientDATA.shares = [];
	ClientDATA.ClearOrders();
	
}
function UserJoinEvent(EVENTID)
{
	var event = ClientDATA.GetEvent(EVENTID);
	if(event != undefined && ClientDATA.CheckMyEventsForEventID(EVENTID) != true){
		var data = [event.AgentID , event.EventID , 0 , ClientDATA.UserID];
		socket.emit('USER_REQUEST_JOIN_EVENT' , data);
	}
}
function UserCreateBUYOrder(EVENTID , COMPETITORID , SHAREQTY , PRICE)
{	
		console.log("UserID: " + ClientDATA.UserID);
		//console.log("EID: " + EVENTID " | COMPID: " + COMPETITORID + " | SHAREQTY: " + SHAREQTY + " | " + PRICE);
		var event = ClientDATA.GetEvent(EVENTID);
		var MyEvent = ClientDATA.GetMyEvent(EVENTID);
		var clientID = ClientDATA.UserID;
		var data1 = [event.AgentID , MyEvent.eventID , MyEvent.grpID , clientID , COMPETITORID , SHAREQTY , PRICE];
		socket.emit('USER_CREATE_BUY_ORDER' , data1);
}
function UserCreateSELLOrder(EVENTID , COMPETITORID , SHAREQTY , PRICE)
{
	console.log("UserID" + ClientDATA.UserID);
	console.log("EVentID" + EVENTID);
	//console.log("EID: " + EVENTID " | COMPID: " + COMPETITORID + " | SHAREQTY: " + SHAREQTY + " | " + PRICE);
	if(SHAREQTY <= ClientDATA.GetMyShareQty(COMPETITORID))
	{
		var event = ClientDATA.GetEvent(EVENTID);
		var MyEvent = ClientDATA.GetMyEvent(EVENTID);
		var clientID = ClientDATA.UserID;
		var data1 = [event.AgentID , MyEvent.eventID , MyEvent.grpID , clientID , COMPETITORID , SHAREQTY , PRICE];
		socket.emit('USER_CREATE_SELL_ORDER' , data1);
	}
}
function UserModifyOrder(ORDERID , ACTION , QTY)
{
	console.log(" -----------------------MODIFIYING ORDER-----------------");
	console.log("OrderID: " + ORDERID);
	console.log("Action: " + ACTION);
	console.log("QTY: " + QTY);
	var event = ClientDATA.GetEventForOrder(ORDERID);
	var order = event.GetOrder(ORDERID);
	var eventID = event.EventID;
	var MyEvent = ClientDATA.GetMyEvent(eventID);
	var event = ClientDATA.GetEvent(eventID);
	if(order.OrderID == ORDERID)
	{
		console.log(ClientDATA.UserID);
		var clientID = ClientDATA.UserID;
		var data1 = [event.AgentID , MyEvent.eventID , MyEvent.grpID , clientID , order.OrderID , ACTION , QTY];
		console.log(data1);
		socket.emit('USER_MODIFY_ORDER' , data1);
	}
	console.log(" -----------------------/MODIFIYING ORDER-----------------");	
}
function UserRemoveOrder(ORDERID)
{
	console.log("OrderID: " + ORDERID);
	var event = ClientDATA.GetEventForOrder(ORDERID);
	var order = event.GetOrder(ORDERID);
	order.status = ORDER_STATUS_CANCELED;
	_main.forceClientRefresh();
	var eventID = event.EventID
	var MyEvent = ClientDATA.GetMyEvent(eventID);
	var event = ClientDATA.GetEvent(eventID);
	if(order.OrderID == ORDERID)
	{
		var data1 = [event.AgentID , MyEvent.eventID , MyEvent.grpID , ClientDATA.UserID , order.OrderID];
		socket.emit('USER_REMOVE_ORDER' , data1);
	}
}
socket.on('PING' , function(data)
{
	var clientID = data[0];
	if(ClientDATA.UserID == clientID)
	{
	  data = [ClientDATA.UserID , ClientDATA.pingChallenge];
	  socket.emit('PONG' , data);
	}
});
socket.on('SERVER_ASSIGNED_CLIENT_ID_FOR_SESSION' , function(data)
{
	ClientDATA.UserID = data[0];
	//_main.forceClientRefresh();
	console.log("UserID:" + ClientDATA.UserID);
	var data = [0 , 0 , ClientDATA.UserID , 0];
	console.log("CLIENT_REQUEST_UPDATE: " + data);
	socket.emit('CLIENT_REQUEST_UPDATE', data);
});
socket.on('EVENT_PLAYER_LOADED_APP_ACK' , function(data)
{
	console.log('Received ack on loading app');
	console.log(data);
	    /*
     * AgentID
     * eventCount
     * EventID
     * competitorCount
     * CompetitorID1
     * CompetitorID2
     * ComeptitorIDn
	 ----Repeat Step for next event... until == EventCount
     * EventStartTime-EpochBased
     */
	 //ClientDATA.events.splice(0, ClientDATA.events.length);
	 var agentID = data[0];
	 var eventCount = data[1];
	 var dataPos = 2;
	 for(var i = 0; i < eventCount; i++)
	 {
		var t_event = new Event();
		t_event.EventID = data[dataPos];
		//var Group1 = new Group();
		dataPos++;
		var competitorCount = data[dataPos];
		dataPos++;
		t_event.AgentID = agentID;
		t_event.sportleague = 0;
		for(var x = 0; x < competitorCount; x++)
		{
			var t_competitor = new Competitor();
			t_competitor.CompetitorID = data[dataPos];
			dataPos++;
			t_competitor.MarketLineHI = data[dataPos];
			dataPos++;
			t_competitor.sharesAtHI = data[dataPos];
			dataPos++;
			t_competitor.MarketLineLOW = data[dataPos];
			dataPos++;
			t_competitor.sharesAtLOW = data[dataPos];
			dataPos++;
			t_event.Competitors.push(t_competitor);
			//Group1.Competitors.push(t_competitor);
		}
		var groupCount = data[dataPos];
		console.log('GroupCount: ' + groupCount);
		dataPos++;
		for(var xx = 0; xx < groupCount; xx++)
		{
			var group = new Group();
			group.GroupID = data[dataPos];
			console.log('GroupID: ' + group.GroupID);
			dataPos++;
			group.PlayerCount = data[dataPos];
			console.log('PlayerCount: ' + group.PlayerCount);
			dataPos++;
			var t_competitorCount1 = data[dataPos];
			dataPos++;
			for(var t = 0; t < t_competitorCount1; t++)
			{
				var t_competitor = new Competitor();
				t_competitor.CompetitorID = data[dataPos];
				dataPos++;
				t_competitor.MarketLineHI = data[dataPos];
				dataPos++;
				t_competitor.sharesAtHI = data[dataPos];
				dataPos++;
				t_competitor.MarketLineLOW = data[dataPos];
				dataPos++;
				t_competitor.sharesAtLOW = data[dataPos];
				dataPos++;
				group.Competitors.push(t_competitor);
				
			}
			t_event.groups.push(group);
		}
		
		
		t_event.groupCount = groupCount;
		t_event.eventPlayerCount =  data[dataPos];
		dataPos++;
		dataPos++;
		t_event.start = data[dataPos];
		if(t_event.start > 0)
		{
			t_event.start *= 1000;
		}
		dataPos++;
		//t_event.start = 1348467927855;
		//                1349467200000
		//                1349467200
		//                1349505372
		//                1349478519
		//                1349503719
		//                1349219319
		//                1349246017
		//                1349472854
		//REMOVE THIS LATER !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		
		//var grpID = 0;
		//Group1.GroupID = grpID;
		//grpID++;
		//Group1.GroupPlayers = [MyPlayer , OtherPlayer];
		//t_event.groups.push(Group1);
		//////////////////////////////////////////////////////////////
		ClientDATA.RemoveEvent(t_event.EventID);
		ClientDATA.events.push(t_event);
		_main.forceClientRefresh();
	 }
	
	
});
socket.on('ADD_ORDER' , function(data)
{
	var eventID = data[0];
	var clientID = data[1];
	var orderID = data[2];
	var orderType = data[3];
	var competitorID = data[4];
	var qty = data[5];
	var price = data[6];
	var status = data[7]
	var time = data[8] * 1000;
	
	var order = new Order();
	order.OrderID = orderID;
	order.OrderType = orderType; //BUY OR SELL
	order.OwnerID = clientID;
	order.CompetitorID = competitorID;
	order.size = qty;
	order.price = price;
	order.status = status;
	order.time = time;
	
	if(ClientDATA.UserID == clientID)
	{
		var event = ClientDATA.GetEvent(eventID);
		if(event != undefined)
		{
			event.AddOrder(order);
			_main.forceClientRefresh();
		}
	}

	
});
socket.on('UPDATE_ORDER_QTY' , function(data)
{
	var eventID = data[0];
	var orderID = data[1];
	var clientID = data[3];
	var qty = data[4];
	console.log(data);
	if(ClientDATA.UserID == clientID)
	{
		var event = ClientDATA.GetEvent(eventID);
		var order = event.GetOrder(orderID);
		order.size = qty;
		_main.forceClientRefresh();
	}
});
socket.on('UPDATE_ORDER__PRICE' , function(data)
{
	var eventID = data[0];
	var orderID = data[1];
	var clientID = data[3];
	var price = data[4];
	console.log(data);
	if(ClientDATA.UserID == clientID)
	{
		var event = ClientDATA.GetEvent(eventID);
		var order = event.GetOrder(orderID);
		order.price = price;
		_main.forceClientRefresh();
	}
});
socket.on('REMOVE_ORDER' , function(data)
{
	eventID = data[0];
	clientID = data[1];
	orderID = data[2];
	/*orderType = data[3];
	
	if(orderType == ORDER_TYPE_BUY)
	{
		var playereventBalance = data[4];
		var event = ClientDATA.GetEvent(eventID);
		event.EventBalance += playereventBalance;
	}
	if(orderType == ORDER_TYPE_SELL)
	{
		var compID = data[4];
		var qty = data[5];
		
		var share = ClientDATA.GetShare(compID);
		share.quanity +=qty;
	}*/
	
	if(ClientDATA.UserID == clientID)
	{
		var event = ClientDATA.GetEvent(eventID);
		var order = event.GetOrder(orderID);
		order.status = ORDER_STATUS_CANCELED;
		
		_main.forceClientRefresh();
		
	}
});
socket.on('EVENT_REMOVED' , function(data){
	var eventID = data[0];
	ClientDATA.RemoveEvent(eventID);
	_main.forceClientRefresh();
});
socket.on('UPDATE_EVENTS' , function(data){
	console.log('EVENTS BEING UPDATE');
	  /*
     * AgentID
     * eventCount
     * EventID
     * competitorCount
     * CompetitorID1
     * CompetitorID2
     * ComeptitorIDn
	 ----Repeat Step for next event... until == EventCount
     * EventStartTime-EpochBased
     */
	 //ClientDATA.events.splice(0, ClientDATA.events.length);
	 var agentID = data[0];
	 var eventCount = data[1];
	 var dataPos = 2;
	  for(var i = 0; i < eventCount; i++)
	 {
		var t_event = new Event();
		t_event.EventID = data[dataPos];
		//var Group1 = new Group();
		dataPos++;
		var competitorCount = data[dataPos];
		dataPos++;
		t_event.AgentID = agentID;
		t_event.sportleague = 0;
		for(var x = 0; x < competitorCount; x++)
		{
			var t_competitor = new Competitor();
			t_competitor.CompetitorID = data[dataPos];
			dataPos++;
			t_competitor.MarketLineHI = data[dataPos];
			dataPos++;
			t_competitor.sharesAtHI = data[dataPos];
			dataPos++;
			t_competitor.MarketLineLOW = data[dataPos];
			dataPos++;
			t_competitor.sharesAtLOW = data[dataPos];
			dataPos++;
			t_event.Competitors.push(t_competitor);
			//Group1.Competitors.push(t_competitor);
		}
		
		var groupCount = data[dataPos];
		console.log('GroupCount' + groupCount);
		dataPos++;
		for(var xx = 0; xx < groupCount; xx++)
		{
			var group = new Group();
			group.GroupID = data[dataPos];
			dataPos++;
			console.log('GroupID: ' + group.GroupID);
			group.PlayerCount = data[dataPos];
			dataPos++;
			console.log('PlayerCount: ' + group.PlayerCount);
			var t_competitorCount1 = data[dataPos];
			dataPos++;
			for(var t = 0; t < t_competitorCount1; t++)
			{
				var t_competitor = new Competitor();
				t_competitor.CompetitorID = data[dataPos];
				dataPos++;
				t_competitor.MarketLineHI = data[dataPos];
				dataPos++;
				t_competitor.sharesAtHI = data[dataPos];
				dataPos++;
				t_competitor.MarketLineLOW = data[dataPos];
				dataPos++;
				t_competitor.sharesAtLOW = data[dataPos];
				dataPos++;
				t_competitor.price = data[dataPos];
				dataPos++;
				group.Competitors.push(t_competitor);
				
			}
			var leaderboard = group.LeaderBoard;
			leaderboard.splice(0 , leaderboard.size);
			var leaderboardSize = data[dataPos];
			dataPos++;
			console.log("Leaderboard being loaded with " + leaderboardSize + " entries");
			for(var xx = 0; xx < leaderboardSize; xx++)
			{
				var playerID = data[dataPos];
				dataPos++;
				var score = data[dataPos];
				dataPos++;
				var t_LeaderBoardEntry = new LeaderBoardEntry();
				t_LeaderBoardEntry.playerID = playerID;
				t_LeaderBoardEntry.playerScore = score;
				leaderboard.push(t_LeaderBoardEntry);
			}
			t_event.groups.push(group);
		}
		
		
		t_event.groupCount = groupCount;
		t_event.eventPlayerCount =  data[dataPos];
		dataPos++;
		dataPos++;
		t_event.start = data[dataPos];
		dataPos++;
		if(t_event.start > 0)
		{
			t_event.start *= 1000;
		}
		var t_success = ClientDATA.UpdateEvent(t_event);
		if(t_success == false){
			ClientDATA.RemoveEvent(t_event.EventID);
			ClientDATA.events.push(t_event);
		}
		_main.forceClientRefresh();
	 }
});
socket.on('USER_LOGIN_SUCCESS' , function(data)
{
	console.log("USER_LOGIN_SUCCESS");
	var newClientID = data[0];
	ClientDATA.UserID = newClientID;
	if(newClientID == ClientDATA.UserID)
	{
		console.log("Client matches");
		console.log(data);
		ClientDATA.UserScore   = data[1];
		ClientDATA.UserBalance = data[2];
		var eventCount = data[3];
		var readPos = 4;
		ClientDATA.MyEvents.splice(0 , ClientDATA.MyEvents.length);
		for(var i = 0; i < eventCount; i++)
		{
			var MyEvent = new MyEvents();
			MyEvent.eventID = data[readPos];
			readPos++;
			MyEvent.grpID = data[readPos];
			readPos++;
			MyEvent.eventBalance = data[readPos];
			readPos++;
			ClientDATA.MyEvents.push(MyEvent);
		}
		var shareCounter = data[readPos];
		readPos++;
		ClientDATA.shares.splice(0 , ClientDATA.shares.length);
		for(var ii = 0; ii < shareCounter; ii++)
		{
			var share = new Shares();
			share.competitorID = data[readPos];
			readPos++;
			share.quanity = data[readPos];
			readPos++;
			share.eventID = data[readPos];
			readPos++;
			share.grpID = data[readPos];
			readPos++;
			ClientDATA.shares.push(share);
		}
		//readPos++;
		var t_orderSize = data[readPos];
		readPos++;
		console.log("OrderCount: "  + t_orderSize);
		if(t_orderSize > 0)
		{
			for(var x = 0; x < t_orderSize; x++)
			{
				var orderID = data[readPos];
				readPos++;
				var eventID = data[readPos];
				readPos++;
				var grpID = data[readPos];
				readPos++;
				var ownerid = data[readPos];
				readPos++;
				var competitorID = data[readPos];
				readPos++;
				var orderType = data[readPos];
				readPos++;
				var qty = data[readPos];
				readPos++;
				var price = data[readPos];
				readPos++;
				var status = data[readPos];
				readPos++;
				var time = data[readPos];
				readPos++;
	console.log("OID: " + orderID + " | EID: " + eventID + " | GID: " + grpID + " | OWNID: " + ownerid);
	console.log("COMPID: " + competitorID + "  | OrderType: " + orderType + " | QTY: " + qty + " | PRICE: " + price + " | TIME: " + time);

				var order = new Order();
				order.OrderID = orderID;
				order.OrderType = orderType; //BUY OR SELL
				order.OwnerID = ownerid;
				order.CompetitorID = competitorID;
				order.size = qty;
				order.status = status;
				order.price = price;
				order.time = (time * 1000);
				console.log(order);
				var event = ClientDATA.GetEvent(eventID);
				//event.orders.splice(0 , event.orders.length);
				event.orders.push(order);
			}
		}
		_main.loginResult(true);
		//_main.clientForceRefresh();
	}
	else
	{
		console.log("Invalid Client ID on login")
	}
});
socket.on('UPDATE_CLIENT_DATA' , function(data)
{
	if(data[0] == ClientDATA.UserID)
	{
		//console.log("Client matches");
		//console.log(data);
		ClientDATA.UserScore   = data[1];
		ClientDATA.UserBalance = data[2];
		var eventCount = data[3];
		var readPos = 4;
		ClientDATA.MyEvents.splice(0 , ClientDATA.MyEvents.length);
		for(var i = 0; i < eventCount; i++)
		{
			var MyEvent = new MyEvents();
			MyEvent.eventID = data[readPos];
			var event = ClientDATA.GetEvent(data[readPos]);
			event.orders.splice(0 , event.orders.length);
			readPos++;
			MyEvent.grpID = data[readPos];
			readPos++;
			MyEvent.eventBalance = data[readPos];
			readPos++;
			
			ClientDATA.MyEvents.push(MyEvent);

		}
		
		var shareCounter = data[readPos];
		readPos++;
		ClientDATA.shares.splice(0 , ClientDATA.shares.length);
		for(var ii = 0; ii < shareCounter; ii++)
		{
			var share = new Shares();
			share.competitorID = data[readPos];
			readPos++;
			share.quanity = data[readPos];
			readPos++;
			share.eventID = data[readPos];
			readPos++;
			share.grpID = data[readPos];
			readPos++;
			ClientDATA.shares.push(share);
		}
		//readPos++;
		var t_orderSize = data[readPos];
		readPos++;
		//console.log("OrderCount: "  + t_orderSize);
		
		if(t_orderSize > 0)
		{
			for(var x = 0; x < t_orderSize; x++)
			{
				var orderID = data[readPos];
				readPos++;
				var eventID = data[readPos];
				readPos++;
				var grpID = data[readPos];
				readPos++;
				var ownerid = data[readPos];
				readPos++;
				var competitorID = data[readPos];
				readPos++;
				var orderType = data[readPos];
				readPos++;
				var qty = data[readPos];
				readPos++;
				var price = data[readPos];
				readPos++;
				var status = data[readPos];
				readPos++;
				var time = data[readPos];
				readPos++;
	//console.log("OID: " + orderID + " | EID: " + eventID + " | GID: " + grpID + " | OWNID: " + ownerid);
	//console.log("COMPID: " + competitorID + "  | OrderType: " + orderType + " | QTY: " + qty + " | PRICE: " + price + " | TIME: " + time);

				var order = new Order();
				order.OrderID = orderID;
				order.OrderType = orderType; //BUY OR SELL
				order.OwnerID = ownerid;
				order.CompetitorID = competitorID;
				order.size = qty;
				order.price = price;
				order.status = status;
				order.time = (time * 1000);
				//console.log(order);
				var event = ClientDATA.GetEvent(eventID);
				//event.orders.splice(0 , event.orders.length);
				//console.log(event);
				//console.log(eventID);
				event.orders.push(order);
			}
		}
		}
		_main.forceClientRefresh();
});
socket.on('USER_LOGIN_FAIL' , function(data)
{
	var clientID = data[0];
	if(ClientDATA.UserID == clientID){
	ClientDATA.MyEvents = [];
	ClientDATA.Username = undefined;
	_main.loginResult(false);
	}
});
socket.on('CLIENT_JOINED_EVENT' , function(data)
{
	console.log('CLIENT_JOINED_EVENT');
	var clientID = data[0];
	var eventID = data[1];
	var groupID = data[2];
	var eventBalance = data[3];
	var shareCount = data[4];
	readPos = 5;
	if(ClientDATA.UserID == clientID)
	{
		console.log('Updating Event Info for joined event');
		var t_myEvent = new MyEvents();
		t_myEvent.eventID = eventID;
		t_myEvent.grpID = groupID;
		t_myEvent.eventBalance = eventBalance;
		ClientDATA.MyEvents.push(t_myEvent);
		//ClientDATA.shares.splice(0 , ClientDATA.shares.length);
		for(var ii = 0; ii < shareCount; ii++)
		{
			var t_share = new Shares();
			t_share.competitorID = data[readPos];
			readPos++;
			t_share.quanity = data[readPos];
			readPos++;
			t_share.eventID = eventID;
			ClientDATA.shares.push(t_share);
		}
		_main.joinResult(eventID);
	}
	
});
socket.on('disconnect' , function(){
	console.log('User disconnected');
	UserLogout();
});
socket.on('connecting', function (){
	 console.log('connecting');
	 ClientDATA.events.splice(0 , ClientDATA.events.length);
	 
});
socket.on('error', function() {
	 console.log('error');
});
socket.on('connect_failed', function() {
	console.log('connect failed');
});

socket.on('USER_REQUEST_ORDER_ACK', function(data){
});
socket.on('USER_REQUEST_LOGOUT_ACK' , function(data){
});
socket.on('USER_REQUEST_JOIN_EVENT_ACK' , function(data){
});
socket.on('USER_REQUEST_UPDATE_DATA_ACK' , function(data){
});
socket.on('USER_REQUEST_UPCOMING_EVENTS_ACK' , function(data){
});
socket.on('USER_REQUEST_MY_EVENTS_ACK' , function(data){
});




//TEST DATA HERE WILL BE REMOVED LATER WHEN SERVER IS FINNALLY CONNECETED
//The Event will be generatred diffrently later but I want to give you a event -- it will still function the same so when I 
//change it you should not have to change anything.
/*
var order1 = new Order();
var order2 = new Order();
var order3 = new Order();
var order4 = new Order();
var order5 = new Order();

order1.OrderID = 11;
order1.TeamID = 1;
order1.OrderType = 'BUY';
order1.size = 2;
order1.price = 13;

order2.OrderID = 12;
order2.OrderType = 'SELL';
order2.TeamID = 1;
order2.size = 2;
order2.price = 17;

order3.OrderID = 13
order3.OrderType = 'SELL';
order3.TeamID = 2;
order3.size = 10;
order3.price = 1;

order4.OrderID = 14;
order4.TeamID = 2;
order4.OrderType = 'BUY';
order4.size = 1;
order4.price = 90;

order5.OrderID = 15;
order5.OrderType = 'SELL';
order5.TeamID = 2;
order5.size = 1
order5.price = 100;

var competitor1 = new Competitor();
competitor1.CompetitorID = 4;
competitor1.CompetitorName = 'nonefornow';
competitor1.MarketLineLOW = 55;
competitor1.MarketLineHI = 73;

var competitor2 = new Competitor();
competitor2.CompetitorID = 8;
competitor2.CompetitorName = 'nonefornow';
competitor2.MarketLineLOW = 12;
competitor2.MarketLineHI = 17;

var competitor3 = new Competitor();
competitor3.CompetitorID = 1;
competitor3.CompetitorName = 'nonefornow';
competitor3.MarketLineLOW = 87;
competitor3.MarketLineHI = 100;

var competitor4 = new Competitor();
competitor4.CompetitorID = 2;
competitor4.CompetitorName = 'nonefornow';
competitor4.MarketLineLOW = 34;
competitor4.MarketLineHI = 89;


var Group1 = new Group();
Group1.GroupID = 1000000;
Group1.GroupPlayers = [MyPlayer , OtherPlayer];
Group1.Competitors = [competitor1 , competitor2];

var Group2 = new Group();
Group2.GroupID = 92434;
Group2.GroupPlayers = [MyPlayer, OtherPlayer];
Group2.Competitors = [competitor3 , competitor4];

var event0 = new Event();
event0.EventID = 10000;
//event0.teams = [ competitor1 , competitor2]; //LETS SAY TEAM 1 IS PADRES and TEAM TWO is ANGELS.... I noticed you ahd those two emblems
event0.sportleague = 0; //lets say 0 is MLB :)
event0.orders = [order1 , order2 , order3 , order4 , order5];
event0.groups = [Group1]; 
event0.start = 0;

var event1 = new Event();
event1.EventID = 10001;
//event1.Competitors = [ competitor3 , competitor4]; //LETS SAY TEAM 1 IS PADRES and TEAM TWO is ANGELS.... I noticed you ahd those two emblems
event1.sportleague = 0; //lets say 0 is MLB :)
event1.groups = [Group2];
event1.start = 1348467927855;*/
console.log(ClientDATA);