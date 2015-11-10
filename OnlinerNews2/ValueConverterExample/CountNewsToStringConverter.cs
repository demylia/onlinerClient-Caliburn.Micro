using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace OnlinerNews.ValueConverterExample
{
	public class CountNewsToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			const string found = "Найдено ",
						   zeroNews = "Новостей не найдено",
							oneNews = "Найдена 1 новость",
					twoTreefourNews = " новости",
					fiveAndMoreNews = " новостей";
							
			int count = (int)value;
			switch (count)
			{
				case 0:
					return zeroNews;
				case 1:
					return oneNews;
				case 2:
				case 3:
				case 4:
					return found + count.ToString() + twoTreefourNews;
				
				default:
					return found + count.ToString() + fiveAndMoreNews;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
