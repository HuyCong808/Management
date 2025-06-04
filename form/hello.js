const mysql = require('mysql2');
const fs = require('fs');
const path = require('path');

// Tạo kết nối
const connection = mysql.createConnection({
    host: '127.0.0.1',
    user: 'root',
    password: '123456789',
    database: 'sakila'
});

// Kết nối tới database
connection.connect((err) => {
    if (err) {
        return console.error('Lỗi kết nối: ' + err.message);
    }
    console.log('Kết nối MySQL thành công!');
});

// Thực hiện truy vấn mẫu
connection.query('SELECT * FROM sakila.actor LIMIT 5', (err, results, fields) => {
    if (err) {
        console.error(err);
        connection.end();
        return;
    }

    // Tạo dòng tiêu đề từ tên các cột
    const headers = fields.map(field => field.name).join(',') + '\n';

    // Tạo dữ liệu từ các dòng truy vấn
    const rows = results.map(row => {
        return fields.map(field => {
            let value = row[field.name];
            if (typeof value === 'string') {
                value = `"${value.replace(/"/g, '""')}"`; // Escape dấu "
            }
            return value;
        }).join(',');
    }).join('\n');

    const csvContent = headers + rows;

    // Ghi ra file
    const filePath = path.join(__dirname, 'actors.csv');
    fs.writeFile(filePath, csvContent, (err) => {
        if (err) {
            console.error('Lỗi ghi file:', err);
        } else {
            console.log('Đã ghi file CSV thành công:', filePath);
        }
    });

    connection.end();
});
