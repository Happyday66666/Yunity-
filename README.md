
# 大樓管理系統｜Yunity 專案

Yunity 是一套以 ASP.NET Core MVC 開發的大樓管理前後台系統，提供管理住戶帳號、公司資料、角色權限與預約功能，並支援帳號驗證、QR Code 生成、非同步操作等進階功能。

## 💡 我負責的功能模組

- ✅ **自訂 ASP.NET Core Identity**
  - 增加 `Role` 欄位，區分身分：住戶、管理員、廠商
  - 登入後根據角色導向不同頁面
- ✅ **Gmail SMTP 帳戶驗證與密碼復原**
  - 使用 Gmail 寄送註冊信、重設密碼信
  - 註冊流程：
    - 使用者自行註冊：自動寄送驗證信
    - 管理員手動註冊：系統寄送含密碼的驗證信
- ✅ **住戶公設預約系統**
  - AJAX 非同步更新預約時間、剩餘人數
  - 實作 CRUD 操作
  - 使用 EF 與 SQL 進行一對多、多對多資料查詢
- ✅ **QR Code 生成模組**
  - 為住戶產生 QR Code 作為身分驗證或報到使用
- ✅ **外部 API 串接**
  - **氣象 API**：串接中央氣象局或 OpenWeatherMap 顯示即時天氣資訊
  - **Google 地圖 API**：於住戶或公司頁面中嵌入地圖並顯示位置資訊

## 🏢 系統功能

- 使用者登入／註冊／角色指派（Admin / Manager / Resident / Vendor）
- 管理公司、使用者帳號、住戶資料
- 公設預約與時段控管
- Email 驗證機制與密碼重設流程
- QR Code 生成與下載功能

## 🔧 技術架構

- ASP.NET Core 8 MVC
- Entity Framework Core + SQL Server
- ASP.NET Identity 使用者與角色管理
- Gmail SMTP (帳號驗證)
- AJAX 非同步互動
- Bootstrap 5 響應式 UI



