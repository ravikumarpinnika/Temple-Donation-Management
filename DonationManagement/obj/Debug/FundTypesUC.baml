   M S B A M L   `   `   `  ����M  IDonationManagement, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:DonationManagementDonationManagement  5      $    DonationManagement.FundTypesUC   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      2local clr-namespace:DonationManagement     5        � DataContext  5	      "   DonationManagement.FundTypes  5
   
   .��5      � $��0,0,0,0q�6   .+�5   
   �� $��Fund Type :��6   $
��Left=�6&   $��15,6,0,0q�6A   $	��Top=�6S   $��88��6k   �� 5   
   $
��Left=�6   $��24��6-   $��	108,8,0,0q�6:    �� 	MaxLength$	 100��6M    �� TextWrapping$
 Wrap=�6]   $	��Top=�6�   $	��356��6�    J� RenderTransformOrigin$ 
0.52,1.413&�6�   ��6q   �� *
FundType+ �� Mode$ TwoWay=�6�   �� 5   
   $��Acc No:��6   $
��Left=�6"   $��	15,51,0,0q�6=   $	��Top=�6P   $��88��6h   �� 5   
   $
��Left=�6   $��24��6-   $��
108,53,0,0q�69   $	 100��6M   $
 Wrap=�6]   $	��Top=�6�   $	��356��6�   $ 
0.52,1.413&�6�   ��6q   �� *	AccountNo+$ TwoWay=�6�   �� 5   
   $��	Comments:��6   $
��Left=�6$   $��	15,96,0,0q�6?   $	��Top=�6R   $��88��6j   �� 5   
   $
��Left=�6   $��24��6-   $��
108,98,0,0q�69   $	 100��6M   $
 Wrap=�6]   $	��Top=�6�   $	��356��6�   $ 
0.52,1.413&�6�   ��6q   �� *
Comments+$ TwoWay=�6�   �� 5   
   $��	Bank Name��6   $
��Left=�6$   $��
15,140,0,0q�6?   $	��Top=�6S   $��88��6k   �� 5   
   $
��Left=�6   $��24��6-   $��108,142,0,0q�69   $	 100��6N   $
 Wrap=�6^   $	��Top=�6�   $	��356��6�   $ 
0.52,1.413&�6�   ��6r   �� *
BankName+$ TwoWay=�6�   �� 5   
   $��
IFSC Code:��6   $
��Left=�6%   $��
15,189,0,0q�6@   $	��Top=�6T   $��88��6l   �� 5   
   $
��Left=�6   $��24��6-   $��108,191,0,0q�69   $	 100��6N   $
 Wrap=�6^   $	��Top=�6�   $	��356��6�   $ 
0.52,1.413&�6�   ��6r   �� *
IFSCCode+$ TwoWay=�6�   �� 5   
   -   5        $��btnSave��5       $
��Save��6   $
��Left=�6/   $��307,231,0,0q�6J   $	��Top=�6_   $��75��6w   6�   �� 5   
   -   5        $��	btnCancel��5   "   $��Cancel��6   $
��Left=�63   $��389,231,0,0q�6N   $	��Top=�6c   $��75��6{   6�   5      5      