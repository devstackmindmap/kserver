module.exports = (function () {
  return {
    local: { // localhost
      host: 'localhost',
      port: '3306',
      user: 'root',
      password: 'alaighdi54$%',
      database: 'knightrun'
    },
    real: { // real server db info
      host: 'localhost',
      port: '3306',
      user: 'root',
      password: '',
        database: ''
    },
    dev1: { // dev server db info
      host: '',
      port: '',
      user: '',
      password: '',
      database: ''
    },
   
  }
})();




