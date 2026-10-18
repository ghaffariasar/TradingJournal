# 💹 FxArchive - ژورنال تخصصی و آنالیز معاملات

سیستم FxArchive یک ابزار حرفه‌ای و با کارایی بالا برای ثبت و تحلیل معاملات در بازارهای فارکس، سهام و کریپتوکارنسی است. این پروژه فراتر از یک ثبت ساده عمل کرده و بینش عمیقی از روانشناسی معامله‌گری، مدیریت ریسک و عملکرد آماری ارائه می‌دهد.

این سیستم با استفاده از **ASP.NET Core 8.0** توسعه یافته و بستری امن، پایدار و کاملاً واکنش‌گرا (Responsive) را برای معامله‌گرانی که به دنبال رشد مستمر هستند فراهم می‌کند.

---

**[English Version / نسخه انگلیسی](README.md)** | **[Technical Wiki / مستندات فنی](docs/README.fa.md)**

---

## 🌟 نگاه کلی به قابلیت‌ها

FxArchive به معامله‌گران اجازه می‌دهد تا با معامله‌گری مانند یک کسب‌وکار برخورد کنند. با ترکیب داده‌های کمی (سود و ضرر، چندگانه R) و داده‌های کیفی (احساسات، پترن‌ها)، این ابزار به شما کمک می‌کند تا دقیقاً متوجه شوید چه چیزی در استراتژی شما کار می‌کند و چه چیزی نیاز به اصلاح دارد.

### 📊 آنالیز پیشرفته و داشبورد
- **منحنی سرمایه (Equity Curve)**: رهگیری بصری رشد سرمایه معاملاتی شما.
- **شاخص‌های کلیدی عملکرد (KPI)**: محاسبه لحظه‌ای نرخ برد، ضریب سود، میانگین چندگانه R و بیشترین دراودان (Drawdown).
- **تفکیک عملکرد**: تحلیل نتایج بر اساس سشن معاملاتی، سیستم معاملاتی، پترن و روزهای هفته.
- **تقویم سود و ضرر ماهانه**: نمایش نقشه حرارتی (Heat-map) از عملکرد ماهانه شما.

### 📝 ثبت جامع معاملات (Journaling)
- **ثبت جزئیات ورود**: ثبت قیمت‌های ورود/خروج، کمیسیون، حجم معامله (Lot) و چندین حد سود.
- **مدیریت ریسک**: محاسبه خودکار نسبت ریسک به ریوارد و R-Multiple.
- **پیوست‌های چندرسانه‌ای**: قابلیت آپلود و ذخیره تا ۳ اسکرین‌شات برای هر معامله جهت بازبینی تحلیل تکنیکال.
- **ارزیابی و یادداشت**: بخش‌های متنی غنی برای برنامه‌ریزی معامله و بازبینی پس از بستن آن.

### 🧠 مزیت روانشناسی (Psychology Tracking)
- **ثبت احساسات**: ثبت وضعیت روحی شما در هنگام ورود، در حین معامله و پس از خروج.
- **احساسات در برابر نتیجه**: کشف همبستگی بین احساسات خاص (مانند ترس، طمع، FOMO) با نتایج معاملاتی ضعیف.

### 🌍 ویژگی‌های بین‌المللی
- **پشتیبانی دو زبانه**: پشتیبانی کامل از انگلیسی (LTR) و فارسی (RTL).
- **تقویم‌های هوشمند**: پشتیبانی بومی از انتخابگرهای تاریخ میلادی و شمسی (جلالی).
- **موتور پوسته**: قابلیت تغییر یکپارچه بین حالت‌های روشن (Light) و تیره (Dark).

---

## 🛠️ تکنولوژی‌های مورد استفاده

| بخش | تکنولوژی‌ها |
| :--- | :--- |
| **بک‌اند (Backend)** | .NET 8.0 (C#), ASP.NET Core MVC |
| **پایگاه داده (Database)** | Entity Framework Core, SQL Server |
| **فرانت‌اند (Frontend)** | Bootstrap 5, jQuery, DataTables.net, Select2 |
| **بصری‌سازی** | Chart.js 4.x |
| **امنیت** | BCrypt.Net, Role-Based Access Control (RBAC), CSRF Protection |

---

## 🚦 شروع به کار

### پیش‌نیازها
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (یا نسخه LocalDB)

### نصب و راه‌اندازی
۱. **کلون کردن و پیمایش**:
   ```bash
   git clone https://github.com/yourusername/FxArchive.git
   cd FxArchive
   ```
۲. **تنظیم پایگاه داده**:
   فایل `appsettings.json` را ویرایش کرده و `DefaultConnection` را مطابق سیستم خود تنظیم کنید.
۳. **اعمال مهاجرت‌ها (Migrations)**:
   ```bash
   dotnet ef database update
   ```
۴. **اجرا**:
   ```bash
   dotnet run
   ```
   برنامه در آدرس `https://localhost:5001` در دسترس خواهد بود.

---

## 🛡️ امنیت و مدیریت
- **کنترل ثبت‌نام**: مدیران می‌توانند امکان ثبت‌نام عمومی را از بخش تنظیمات سیستم فعال یا غیرفعال کنند.
<<<<<<< HEAD
- **سیستم مجوزها**: کنترل دقیق بر روی دسترسی کاربران برای مشاهده، ایجاد یا ویرایش معاملات و داده‌های مرجع.
=======
- **سیستم نقش‌ها و مجوزها**: 
    - دارای نقش‌های پیش‌فرض **مدیر سیستم**، **معامله‌گر** و **اپراتور**.
    - کنترل دقیق بر روی دسترسی‌ها (Granular Permissions) از جمله مدیریت حساب‌های شخصی جدا از اطلاعات پایه سیستم.
- **ایزولاسیون داده‌ها**: تضمین عدم دسترسی کاربران به حساب‌ها و معاملات یکدیگر.
>>>>>>> 6ad8cb6 (Update Project)
- **یکپارچگی داده‌ها**: اعتبارسنجی کامل سمت سرور و هشینگ امن رمزهای عبور.

---

<<<<<<< HEAD
=======
## 📱 بهینه‌سازی موبایل
تمامی جداول و صفحات مدیریتی (نمادها، بروکرها، سیستم‌ها، کاربران و ...) برای نمایش در موبایل و تبلت بهینه شده‌اند و از اسکرول افقی هوشمند در جداول بزرگ پشتیبانی می‌کنند.

---

>>>>>>> 6ad8cb6 (Update Project)
## 📸 تصاویر محیط برنامه
*(به زودی - تصاویر محیط برنامه را در اینجا قرار دهید)*

---

## 🤝 مشارکت در پروژه
مشارکت‌های شما باعث می‌شود دنیای متن‌باز (Open Source) مکانی شگفت‌انگیز برای یادگیری و خلق کردن باشد. هرگونه مشارکت شما **بسیار ارزشمند** است.

۱. پروژه را Fork کنید.
۲. یک شاخه برای ویژگی خود بسازید (`git checkout -b feature/AmazingFeature`).
۳. تغییرات خود را Commit کنید (`git commit -m 'Add some AmazingFeature'`).
۴. به شاخه خود Push کنید (`git push origin feature/AmazingFeature`).
۵. یک Pull Request باز کنید.

## ⚖️ لایسنس
این پروژه تحت لایسنس MIT منتشر شده است. برای اطلاعات بیشتر فایل `LICENSE` را ببینید.

---
*توسعه یافته با ❤️ برای جامعه معامله‌گران.*





# 💹 FxArchive - Professional Trading Journal & Analytics

FxArchive is a high-performance, professional-grade trading journal designed for Forex, Stocks, and Crypto traders. It goes beyond simple record-keeping, offering deep insights into trading psychology, risk management, and statistical performance.

Built with **ASP.NET Core 8.0**, it provides a robust, secure, and highly responsive platform for traders who are serious about their edge.

---

**[Persian Version / نسخه فارسی](README.fa.md)** | **[Technical Wiki](docs/README.md)**

---

## 🌟 High-Level Overview

FxArchive empowers traders to treat trading like a business. By combining quantitative data (PnL, R-Multiple) with qualitative data (Emotions, Patterns), it helps identify exactly what works and what doesn't.

### 📊 Advanced Analytics & Dashboard
- **Equity Curve**: Visual growth tracking of your trading capital.
- **KPI Metrics**: Real-time calculation of Win Rate, Profit Factor, Average R-Multiple, and Max Drawdown.
- **Performance Breakdown**: Analyze results by Trading Session, System, Pattern, and Day of Week.
- **Monthly PnL Calendar**: A visual heat-map of your monthly performance.

### 📝 Comprehensive Trade Journaling
- **Detailed Entry**: Record entry/exit prices, fees, lot sizes, and multiple profit targets.
- **Risk Management**: Automatic calculation of Risk-to-Reward ratios and R-Multiple.
- **Multi-Media Attachments**: Upload and store up to 3 screenshots per trade for technical analysis review.
- **Evaluation & Notes**: Rich text areas for trade planning and post-trade reflection.

### 🧠 Psychological Edge (Psychology Tracking)
- **Emotion Logging**: Track how you felt *at entry*, *during the trade*, and *after closing*.
- **Emotion vs. Outcome**: Discover if certain emotions (e.g., Fear, Greed, FOMO) correlate with poor performance.

### 🌍 Global Features
- **Dual Localization**: Full English (LTR) and Persian (RTL) support.
- **Smart Calendars**: Native support for Gregorian and Jalali (Persian) date pickers.
- **Theme Engine**: Seamless switching between Light and Dark modes.

---

## 🛠️ Technology Stack

| Layer | Technologies |
| :--- | :--- |
| **Backend** | .NET 8.0 (C#), ASP.NET Core MVC |
| **Database** | Entity Framework Core, SQL Server (LocalDB/Express/Production) |
| **Frontend** | Bootstrap 5, jQuery, DataTables.net, Select2 |
| **Visuals** | Chart.js 4.x |
| **Security** | BCrypt.Net, Role-Based Access Control (RBAC), CSRF Protection |

---

## 🚦 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or LocalDB)

### Installation & Setup
1. **Clone & Navigate**:
   ```bash
   git clone https://github.com/yourusername/FxArchive.git
   cd FxArchive
   ```
2. **Configure Database**:
   Edit `appsettings.json` and update the `DefaultConnection` string.
3. **Apply Migrations**:
   ```bash
   dotnet ef database update
   ```
4. **Run**:
   ```bash
   dotnet run
   ```
   Access the app at `https://localhost:5001`.

---

## 🛡️ Security & Administration
- **Registration Control**: Admins can toggle public registration via the System Settings.
- **Role-Based Access Control (RBAC)**: 
    - Pre-defined roles for **Admin**, **Trader**, and **Operator**.
    - Granular permissions including separate management for personal trading accounts.
- **Data Isolation**: Strict security filters ensure users can only access their own trades and accounts.
- **Data Integrity**: Full server-side validation and secure password hashing.

---

## 📱 Mobile Optimization
All data tables and management pages (Symbols, Brokers, Systems, Users, etc.) are fully optimized for mobile devices with responsive layouts and horizontal scrolling support.

---

## 📸 Screenshots
*(Coming Soon - Add your screenshots here to showcase the beautiful UI)*

---

## 🤝 Contributing
Contributions are what make the open-source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## ⚖️ License
Distributed under the MIT License. See `LICENSE` for more information.

---
*Developed with ❤️ for the Trading Community.*
