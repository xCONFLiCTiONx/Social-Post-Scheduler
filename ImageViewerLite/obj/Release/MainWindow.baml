   M S B A M L   `   `   `  ���� J  FImageViewerLite, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:ImageViewerLiteImageViewerLite       ImageViewerLite.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     /localclr-namespace:ImageViewerLite     -   $��
mainWindow��  � Title$  ImageViewer�� � WindowStyle$
 None=� � WindowState$ 	Maximized=�$
��1024��$	��768�� � AllowsTransparency .  J� 	AllowDrop . ���	#FFEEEEEE.��� -    �Name$ mainGrid��$��5q� � UseLayoutRounding . .+��� -    ��Name$ border��}�. ._�� .+��� -    ��Name$ ImagePlayer��$��Uniform=�	 �� BitmapScalingMode$	 HighQuality=�$��Center=�$��Center=�r� -   $��	GIFPlayer��
 r� LoadedBehavior$
 Manual=� r� Stretch$ Uniform=�$	 HighQuality=�$��Center=�$��Center=� r� UnloadedBehavior$ Manual=��� -    ��Name$ CloseButton��   CustomButton#��[  $��40��$��40�� � ToolTip$ Close�� ^� InitialShowDelay$
 1000�� ^� ShowDuration$
 2500�� ^� BetweenShowDelay$ 0�� � Cursor$
 Hand��$��Right=�$	��Top=�.��F� $��16��  	CloseIcon#��[  �� Stretch$ Uniform=�����  �� ElementName$ CloseButton�� �� Path$ 
Foreground��� -   $ ZoomInButton��#��[  $��40��$��40��$ Zoom In��$
 1000��$
 2500��$ 0��$
 Hand��$��	1,-90,0,0q�$��Right=�.��F�  J� Opacity$	 0.9Q�$��16��  
ZoomInIcon#��[ $ Uniform=����� $ ZoomInButton��$ 
Foreground��� -   $ ZoomResetButton��#��[  $��40��$��40��$ 
Reset Zoom��$
 1000��$
 2500��$ 0��$
 Hand��$��1,0,0,0q�$��Right=�.��F� $	 0.9Q�$��20��  ZoomResetIcon#��[ $ Uniform=����� $ ZoomResetButton��$ 
Foreground��� -	   $ ZoomOutButton��#��[  $��40��$��40��$ Zoom Out��$
 1000��$
 2500��$ 0��$
 Hand��$��1,90,0,0q�$��Right=�.��F� $	 0.9Q�$��16��  ZoomOutIcon#��[ $ Uniform=����� $ ZoomOutButton��$ 
Foreground��� -
   $ RotateButton��#��[  $��40��$��40��$ Rotate Image��$
 1000��$
 2500��$ 0��$
 Hand��$
��Left=�.��F� $	 0.9Q�$��32��  RotateRightIcon#��[ $ Uniform=����� $ RotateButton��$ 
Foreground�