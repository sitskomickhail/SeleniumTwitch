   M S B A M L   `   `   `  ����A  =TwiBot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:TwiBotTwiBot  5   	       TwiBot.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:TwiBot     5   	   -   5          � Title$  Bots for Twitch.tv��5   	   $	��200��6$   $	��350��61   
���� �� � 
ResizeMode$ NoResize=�5   	    � Icon$% /Icons/iconfinder_16_940984.png��5   	   .��5      �  � RowDefinitions 5   
   �� 5      $
��auto �6   �� 5      $��47 �6   �� 5      $	��48* �6   �� 5      $	��47* �6   .+�5   
   ��  �� Orientation$ 
Horizontal=�6   $��Right=�6.   $	��337��6J   $��0,2,5,0q�6V   .+�5      �� -   5         ��Name$ tbCount��5      $	��Top=�5      $	��229��6.   $��22��6:   $��10, 0, 0, 0q�5      ��5      ��  �� ElementName$ cbCount�� �� Path$ SelectedItem.Content� �� Mode$ OneWay=�5      �� 5      -   5        	 ��Name$	 cbCount��5      $��Right=�5      $	��102��5      $��
0, 0, 0, 5q�5      
 �� SelectedIndex$
 0��5      .@�5      �� $��ALL BOTS��6   �� 5      $��50��6   �� 5       $	��100��6   �� 5!      $	��150��6   �� 5"      $	��200��6   5#      5$      �� 5%   
   -   5        $ tbUrl��5%      $��1��5&       �� TextWrapping$
 Wrap=�5'       �� AcceptsReturn . $
��Bold"�5)      $��14(�5*      $��
5, 0, 5, 0q�5+      �� 5,   
   -   5         ��Name$ btnStart��5,      $��2��5-      $��Start��5.      $��5q�50      �� 51   
   $ 
Horizontal=�6   $��3��6.   .+�52      �� -   5        $ btnGenerate��52      $��2��53      $��add bots info��54      $��5q�55      56      �� 57      -   5        $ btnHelp_Mark��5;      $��Right=�57      $��5q�58      
������ 
������ .��5=      �� $��/Icons/question_mark.png��6   $��25��6:   $��25��6E   5>      �� 5?      -   5        $ btnClose��5A      $��Right=�5?      $��5q�5@      
������ 
������ .��5E      �� $��/Icons/close.png��6   $��25��62   $��25��6=   5F      5G      5H      5I      