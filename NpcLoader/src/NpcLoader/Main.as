package NpcLoader
{
	import flash.display.MovieClip;
	import flash.display.Sprite;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.external.ExternalInterface;
	import flash.net.URLRequest;
	import flash.display.Loader;
	
	public class Main extends Sprite
	{
		private const movieX:int = 500;
		private const movieY:int = 500;
		
		private var myMovieClip:MovieClip;
		private var rectangle:Sprite = new Sprite(); // Dikdörtgen alanı çizecek olan Sprite
		
		private var startX:Number;
		private var startY:Number;
		private var rectWidth:Number;
		private var rectHeight:Number;
		
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
				var x:int = movieX + parseInt(parameters[0]);
				var y:int = movieY + parseInt(parameters[1]);
				var width:int = parameters[2];
				var height:int = parameters[3];
				drawRectangle(x, y, width, height);
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
					
					var x:int = movieX + parseInt(parameters[3]);
					var y:int = movieY + parseInt(parameters[4]);
					var width:int = parameters[5];
					var height:int = parameters[6];
					drawRectangle(x, y, width, height);
				}
				else
				{
					trace("myClass geçersiz");
				}
			});
			loader.load(new URLRequest(swfURL));
			addChild(loader);
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
			startX = event.stageX;
			startY = event.stageY;
			
			stage.addEventListener(MouseEvent.MOUSE_MOVE, onMouseMove);
			stage.addEventListener(MouseEvent.MOUSE_UP, onMouseUp);
			
			drawRectangle(startX, startY, 0, 0);
		}
		
		private function onMouseMove(event:MouseEvent):void
		{
			var width:Number = mouseX - startX;
			var height:Number = mouseY - startY;
			
			drawRectangle(startX, startY, width, height);
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
			
			ExternalInterface.call("newHitbox", (startX - movieX), (startY - movieY), rectangle.width, rectangle.height);
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
			var noktaSprite:Sprite = new Sprite();
			noktaSprite.graphics.beginFill(0xfc03db); // renk
			noktaSprite.graphics.drawCircle(x, y, cap);
			noktaSprite.graphics.endFill();
			addChild(noktaSprite);
		
		}
	}
}
