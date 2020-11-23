library flutter_lib;
import 'package:sqflite/sqflite.dart';
import 'package:path/path.dart';

class SqlHelper {
  Future<String> getPath() async {
    var dbPath = await getDatabasesPath();
    String path = join(dbPath, 'AppDB.db');
    print(path);
    return path;
  }

  Future<Database> openDb() async {
    return await openDatabase(await getPath(), version: 1,
        onCreate: (Database db, int version) async {
      // When creating the db, create the table
      await db.execute(
          'CREATE TABLE IF NOT EXISTS Auth ( "Identifier" TEXT NOT NULL DEFAULT "", "Username" TEXT NOT NULL DEFAULT "", "Expires_In" INTEGER NULL DEFAULT 0, "Token" TEXT NULL DEFAULT "", PRIMARY KEY("Identifier") )');

//      await db.execute(
//          'CREATE TABLE IF NOT EXISTS TouchID ( "Identifier" TEXT NOT NULL DEFAULT "", "Username" TEXT NOT NULL DEFAULT "", "isEnable" BOOLEAN NOT NULL DEFAULT 0, PRIMARY KEY("Identifier") )');
    });
  }

  Future<void> closeDb(Database db) async {
    return await db.close();
  }

  Future<List<Map>> query({String command}) async {
    var db = await openDb();

    var map = await db.rawQuery(command);

    closeDb(db);

    return map;
  }

  Future<int> count({String command}) async {
    var db = await openDb();

    var count = Sqflite.firstIntValue(await db.rawQuery(command));

    closeDb(db);

    return count;
  }

  Future<int> insert({String command}) async {
    var re = 0;

    var db = await openDb();

    await db.transaction((txn) async {
      re = await txn.rawInsert(command);
    });

    closeDb(db);

    return re;
  }

  Future<int> update({String command}) async {
    var re = 0;

    var db = await openDb();

    await db.transaction((txn) async {
      re = await txn.rawUpdate(command);
    });

    closeDb(db);

    return re;
  }

  Future<int> delete({String command}) async {
    var re = 0;

    var db = await openDb();

    await db.transaction((txn) async {
      re = await txn.rawDelete(command);
    });

    closeDb(db);

    return re;
  }

  Future<void> execute({String command}) async {
    var db = await openDb();

    await db.transaction((txn) async {
      await txn.execute(command);
    });

    closeDb(db);
  }
}
