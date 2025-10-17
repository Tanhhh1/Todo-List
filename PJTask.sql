-- Tạo database TodoList
CREATE DATABASE PJTask;
GO

USE PJTask;
GO

-- Bảng Users
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Fullname NVARCHAR(100),
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    Address NVARCHAR(255),
    Birthday DATETIME,
    Phone NVARCHAR(20),
	Avatar NVARCHAR(255),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE()
);
GO

-- Bảng Tasks
CREATE TABLE Tasks (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    Status NVARCHAR(50) DEFAULT 'Pending',
    DueDate DATETIME,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Tasks_Users FOREIGN KEY (UserId)
        REFERENCES Users(Id)
        ON DELETE CASCADE
);
GO

-- Thêm dữ liệu mẫu cho Users
INSERT INTO Users (Username, Fullname, Email, Password, Address, Phone)
VALUES
(N'tanh123', N'Tuấn Anh', N'Tanhhh1@gmail.com', N'$2a$11$eRprk7/bulBZsZ2QPb3ifeQ55F3YvOUIRvilMTQWGUJr6N2Y6spcG', N'Thanh Xuân, Hà Nội', N'0945612378'),
(N'duy123', N'Duy Anh', N'Danh1@gmail.com', N'$2a$11$eRprk7/bulBZsZ2QPb3ifeQ55F3YvOUIRvilMTQWGUJr6N2Y6spcG', N'Thanh Xuân, Hà Nội', N'0123456789')
GO

-- Thêm dữ liệu mẫu cho Tasks
INSERT INTO Tasks (UserId, Title, Description, Status, DueDate)
VALUES
(1, N'Hoàn thành báo cáo dự án', N'Viết và nộp báo cáo tổng kết cho dự án phần mềm', N'Pending', '2025-10-25'),
(1, N'Thuyết trình nhóm', N'Chuẩn bị slide và luyện tập thuyết trình trước khi báo cáo', N'Pending', '2025-10-22'),
(1, N'Kiểm thử ứng dụng', N'Thực hiện test các chức năng chính của ứng dụng web', N'In Progress', '2025-10-28'),
(1, N'Cập nhật hồ sơ cá nhân', N'Cập nhật thông tin cá nhân và ảnh đại diện trong hệ thống', N'Done', '2025-10-10'),
(1, N'Họp nhóm hàng tuần', N'Tham gia cuộc họp nhóm định kỳ để cập nhật tiến độ công việc', N'In Progress', '2025-10-18'),
(1, N'Nghiên cứu công nghệ mới', N'Tìm hiểu về ASP.NET Core 9 và các tính năng mới', N'Done', '2025-11-05'),
(1, N'Viết tài liệu hướng dẫn', N'Viết hướng dẫn cài đặt và sử dụng cho người dùng cuối', N'Pending', '2025-10-30'),
(1, N'Trả lời email khách hàng', N'Phản hồi các email yêu cầu hỗ trợ từ khách hàng', N'In Progress', '2025-10-15'),
(1, N'Bảo trì server', N'Thực hiện kiểm tra và cập nhật bảo mật cho hệ thống server', N'Done', '2025-11-01'),
(1, N'Đánh giá hiệu suất', N'Tự đánh giá kết quả công việc trong quý này', N'Done', '2025-10-27');
GO

INSERT INTO Tasks (UserId, Title, Description, Status, DueDate)
VALUES
(2, N'Mua thực phẩm trong tuần', N'Mua rau, thịt, cá và sữa cho tuần tới', N'Done', '2025-10-19'),
(2, N'Dọn dẹp nhà cửa', N'Dọn phòng ngủ, phòng khách và nhà bếp cuối tuần', N'Pending', '2025-10-20'),
(2, N'Trả tiền điện nước', N'Thanh toán hóa đơn điện và nước trước hạn', N'Pending', '2025-10-15'),
(2, N'Học tiếng Anh online', N'Học 2 bài trong khóa luyện nói tiếng Anh mỗi ngày', N'Pending', '2025-10-22'),
(2, N'Gọi điện cho bố mẹ', N'Thăm hỏi sức khỏe và nói chuyện cùng bố mẹ', N'In Progress', '2025-10-14'),
(2, N'Đọc sách kỹ năng sống', N'Đọc 1 chương sách “7 thói quen của người thành đạt”', N'In Progress', '2025-10-23'),
(2, N'Tập thể dục buổi sáng', N'Chạy bộ 3km quanh công viên gần nhà', N'Done', '2025-10-18'),
(2, N'Nấu ăn cuối tuần', N'Nấu bữa tối đặc biệt cho gia đình', N'Done', '2025-10-26'),
(2, N'Lên kế hoạch du lịch', N'Lập danh sách địa điểm và ngân sách cho chuyến đi Đà Lạt', N'Done', '2025-11-05'),
(2, N'Mua quà sinh nhật bạn', N'Chọn và mua quà cho sinh nhật bạn thân', N'Done', '2025-10-17');
GO