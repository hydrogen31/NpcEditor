package NpcLoader
{
	import flash.display.DisplayObjectContainer;
	import flash.display.MovieClip;
	import flash.display.Sprite;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.external.ExternalInterface;
	import flash.net.URLRequest;
	import flash.display.Loader;
	import flash.display.Bitmap;
	import flash.display.Shape;
	import flash.text.TextField;
	import flash.text.TextFormat;
	
	public class Main extends Sprite
	{
		private var movieX:Number = 500;
		private var movieY:Number = 500;
		
		private var myMovieClip:MovieClip;
		private var rectangle:Sprite = new Sprite(); // Dikdörtgen alanı çizecek olan Sprite
		private var noktaSprite:Sprite = new Sprite();
		
		private var mapContainer:Sprite = new Sprite();
		private var mapPointText:TextField = new TextField();
		
		private var startX:Number;
		private var startY:Number;
		private var rectWidth:Number;
		private var rectHeight:Number;
		
		private const screenWidth:Number = 1000;
		private const screenHeight:Number = 600;
		
		private var Scale:Number = 1;
		
		private var ctrl:Boolean = false;
		
		public function Main()
		{
			if (stage)
				init();
			else
				addEventListener(Event.ADDED_TO_STAGE, init);
		}
		
		private function init(e:Event = null):void
		{
			removeEventListener(Event.ADDED_TO_STAGE, init);
			
			ExternalInterface.addCallback("handleFlashCall", handleFlashCall);
			stage.addEventListener(MouseEvent.MOUSE_DOWN, onMouseDown);
			
			addChild(rectangle);
		}
		
		public function handleFlashCall(command:String, params:String):void
		{
			trace("flash call received. command: " + command);
			var parameters:Array = params.split(",");
			
			switch (command)
			{
			case "loadAndPlay": 
				loadAndPlay(parameters);
				break;
			case "playMovie": 
				if (myMovieClip)
					myMovieClip.doAction(params);
				break;
			case "drawHitbox": 
				drawHitbox(parameters[0], parameters[1], parameters[2], parameters[3]);
				break;
			case "setMap": 
				setMap(parameters);
				break;
			}
		}
		
		private function loadAndPlay(parameters:Array):void
		{
			while (numChildren > 0)
				removeChildAt(0);
			
			var swfURL:String = parameters[0];
			var spriteName:String = parameters[1];
			var targetLabel:String = parameters[2];
			
			var loader:Loader = new Loader();
			loader.contentLoaderInfo.addEventListener(Event.COMPLETE, function(event:Event):void
			{
				var myClass:Class = loader.contentLoaderInfo.applicationDomain.getDefinition(spriteName) as Class;
				if (myClass)
				{
					myMovieClip = new myClass() as MovieClip;
					
					myMovieClip.doAction(targetLabel);
					myMovieClip.x = movieX;
					myMovieClip.y = movieY;
					
					myMovieClip.addEventListener(Event.ADDED_TO_STAGE, onMovieClipAddedToStage);
					addChild(myMovieClip);
					
					drawHitbox(parameters[3], parameters[4], parameters[5], parameters[6]);
				}
				else
				{
					trace("myClass geçersiz");
				}
			});
			loader.load(new URLRequest(swfURL));
			addChild(loader);
			addChild(mapContainer);
		}
		
		private function onMovieClipAddedToStage(event:Event):void
		{
			noktaCiz(movieX, movieY, 3);
			
			var labelNames:Array = [];
			for each (var label:Object in myMovieClip.currentLabels)
				labelNames.push(label.name);
			
			if (ExternalInterface.available)
				ExternalInterface.call("sendLabels", labelNames.join(","));
			
			myMovieClip.removeEventListener(Event.ADDED_TO_STAGE, onMovieClipAddedToStage);
		
		}
		
		private function onMouseDown(event:MouseEvent):void
		{
			startX = mouseX;
			startY = mouseY;
			
			stage.addEventListener(MouseEvent.MOUSE_MOVE, onMouseMove);
			stage.addEventListener(MouseEvent.MOUSE_UP, onMouseUp);
			
			ctrl = event.ctrlKey;
			
			if (!ctrl)
				drawRectangle(startX, startY, 0, 0);
		}
		
		private function onMouseMove(event:MouseEvent):void
		{
			var width:Number = mouseX - startX;
			var height:Number = mouseY - startY;
			
			if (!ctrl)
				drawRectangle(startX, startY, width, height);
			else
			{
				movieX = mouseX;
				movieY = mouseY;
				
				myMovieClip.x = mouseX;
				myMovieClip.y = mouseY;
				
				noktaCiz(mouseX, mouseY, 3);
			}
		}
		
		private function onMouseUp(event:MouseEvent):void
		{
			stage.removeEventListener(MouseEvent.MOUSE_MOVE, onMouseMove);
			stage.removeEventListener(MouseEvent.MOUSE_UP, onMouseUp);
			
			//eksi değer düzeltmeleri
			if (rectWidth < 0)
			{
				startX = startX + rectWidth;
			}
			
			if (rectHeight < 0)
			{
				startY = startY + rectHeight;
			}
			
			if (!event.ctrlKey) //ctrl + drag to move npc
			{
				var x:int = startX - movieX;
				var y:int = startY - movieY;
				var width:int = rectangle.width;
				var height:int = rectangle.height;
				ExternalInterface.call("newHitbox", x, y, width, height);
			}
		}
		
		private function drawHitbox(realX:int, realY:int, realWidth:int, realHeight:int):void
		{
			var x:Number = movieX + (realX);
			var y:Number = movieY + (realY);
			var width:Number = realWidth;
			var height:Number = realHeight;
			
			drawRectangle(x, y, width, height);
		}
		
		private function drawRectangle(x:Number, y:Number, width:Number, height:Number):void
		{
			rectangle.graphics.clear();
			rectangle.graphics.beginFill(0xFFFFFF, 0.3);
			rectangle.graphics.drawRect(x, y, width, height);
			rectangle.graphics.endFill();
			
			rectWidth = width;
			rectHeight = height;
			
			addChild(rectangle);
		}
		
		private function noktaCiz(x:Number, y:Number, cap:Number):void
		{
			noktaSprite.graphics.clear();
			noktaSprite.graphics.beginFill(0xfc03db); // renk
			noktaSprite.graphics.drawCircle(x, y, cap);
			noktaSprite.graphics.endFill();
			addChild(noktaSprite);
		
		}
		
		//Map
		
		private function setMap(parameters:Array):void
		{
			while (mapContainer.numChildren > 0)
				mapContainer.removeChildAt(0);
			
			Scale = 1;
			
			var deadUrl:String = parameters[0];
			var foreUrl:String = parameters[1];
			var posX:String = parameters[2];
			var posX1:String = parameters[3];
			
			loadImage(deadUrl);
			loadImage(foreUrl);
			
			showTeamPlayers(posX, 0xFF0000);
			showTeamPlayers(posX1, 0x0000FF);
			
			mapPointText.x = mapPointText.y = 20;
			mapPointText.textColor = 0xFFFFFF;
			mapPointText.mouseEnabled = false;
			
			mapContainer.addChild(mapPointText);
			stage.addEventListener(MouseEvent.MOUSE_MOVE, onMapMouseMove);
		
		}
		
		private function loadImage(url:String):void
		{
			if (url && url != "")
			{
				var qLoader:Loader = new Loader();
				qLoader.contentLoaderInfo.addEventListener(Event.COMPLETE, OnImageLoad);
				qLoader.load(new URLRequest(url));
				mapContainer.addChild(qLoader);
			}
		}
		
		private function OnImageLoad(e:Event):void
		{
			var qTempBitmap:Bitmap = e.target.loader.content as Bitmap;
			var mapBitmap:Bitmap = new Bitmap(qTempBitmap.bitmapData);
			
			var scale:Number = Math.min(screenWidth / mapBitmap.width, screenHeight / mapBitmap.height);
			RecalcScale(mapBitmap.width, mapBitmap.height);
			scaleX = scaleY = Scale;
			mapPointText.scaleX = mapPointText.scaleY = 3;
			
			mapContainer.addChild(mapBitmap);
			bringPointTextToFront();
			trace("scale: " + scale);
		}
		
		private function RecalcScale(width:int, height:int):void
		{
			if (width > screenWidth)
			{
				Scale = screenWidth / width;
			}
			if (height > screenHeight)
			{
				var scale2:Number = screenHeight / height;
				if (scale2 < Scale)
				{
					Scale = scale2;
				}
			}
		}
		
		private function bringPointTextToFront():void
		{
			var container:DisplayObjectContainer = mapPointText.parent;
			container.setChildIndex(mapPointText, container.numChildren - 1);
		}
		
		private function showTeamPlayers(posX:String, color:uint):void
		{
			var posXarray:Array = posX.split("|");
			
			for each (var pos:String in posXarray)
			{
				var loc:Array = pos.split("-");
				
				var x:Number = parseFloat(loc[0]);
				var y:Number = parseFloat(loc[1]);
				
				var daire:Shape = new Shape();
				daire.graphics.beginFill(color);
				daire.graphics.drawCircle(0, 0, 10);
				daire.graphics.endFill();
				
				daire.x = x;
				daire.y = y;
				
				mapContainer.addChild(daire);
			}
		}
		
		private function onMapMouseMove(event:MouseEvent):void
		{
			mapPointText.text = int(mouseX) + ", " + int(mouseY);
		}
	}
}