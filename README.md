PC tool 包含了
1. type C 線連接後切換 edl 模式
2. QFIL
    a. 可修改 elf 檔案來切換不同燒錄版本
    b. 燒錄進度可透過 tool 視窗得知
    c. 燒錄結果之 log 檔儲存位置可切換
![](ossaTool/Properties/appScreenshot/tab1.png) 

3. 裝置資訊取得
    a. IP address
    b. mac address
    c. device serial number
4. 初始化 RPMB
    若裝置無法取得其 mac address 與 device serial number 無法執行此步驟及後續流程
5. 燒錄金鑰
    a. push 金鑰 xml 檔
    b. push 成功之後需請使用者開啟命令提示字元並按下 ctrl + V 執行
    c. push 成功之後自動更新 csv 檔 (txt 檔可依使用者需求自行決定是否更新)
6. 金鑰資料庫自動管理
    a.選擇資料庫後自動搜尋底下未使用之金鑰
    b.更新 5c 後將已使用之金鑰移至 used 資料夾中並重新抓取新的一組可用金鑰
![](ossaTool/Properties/appScreenshot/tab2.png) 

![](ossaTool/Properties/appScreenshot/tab3.png) 