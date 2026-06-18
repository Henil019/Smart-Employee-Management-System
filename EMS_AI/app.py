from flask import Flask, request, jsonify
import requests
import pyodbc
from flask_cors import CORS 
app=Flask(__name__)
CORS(app)


conn=pyodbc.connect(
"DRIVER={SQL Server};"
"SERVER=.\\SQLEXPRESS;"
"DATABASE=EMS;"
"Trusted_Connection=yes;"
)
cursor=conn.cursor();

@app.route('/ask', methods=['POST'])

def ask():
        data=request.json
        question=data['question'];

        if "how many employees" in question.lower():
            cursor.execute(
                "Select count(*) from Employee"
            )
            count=cursor.fetchone()[0]
            return jsonify({
            "answer":f"There are {count} employees."
            })

        if "highest paid employee" in question.lower():
            cursor.execute(
                "SELECT MAX(Salary) FROM Employee"
            )
            salaryy=cursor.fetchone()[0]
            return jsonify({
            "answer":f"Highest salary {salaryy}."
            })

        if "average salary" in question.lower():
            cursor.execute(
                "SELECT AVG(Salary) FROM Employee"
            )
            avggg=cursor.fetchone()[0]
            return jsonify({
            "answer":f"average salary {avggg}."
            })

        if "list department" in question.lower():
            cursor .execute(
                "SELECT Name FROM Department"
            )
            departments=cursor.fetchall()
            print(departments)
            dept_name=[row[0] for row in departments]
           

            return jsonify({
            "answer":", ".join(dept_name)
            })

        if "employee from it" in question.lower():
                cursor.execute(
                    "SELECT e.FirstName, e.LastName FROM Employee e JOIN Department d ON e.DepartmentId=d.Id WHERE d.Name='IT'"
                )
                rows=cursor.fetchall()

                names=[]
                for row in rows:
                    names.append(f"{row[0]} {row[1]}")
                return jsonify({
                "answer":", ".join(names)
                })

        if "show me total employees" in question.lower():
            cursor.execute(
                "SELECT FirstName, LastName, Email, Salary FROM Employee"
            )
            rows=cursor.fetchall()

            names2=[]
            print(names2)
            for row in rows:
                names2.append(f"{row[0]} {row[1]} {row[2]} {row[3]}")
            return jsonify({
            "answer":", ".join(names2)
            })

        response=requests.post(
         'http://localhost:11434/api/generate',
         json={
               'model':'phi3', 
               'prompt':question,
               'stream':False
              }  
         )
        answer=response.json()['response'];
        return jsonify({
          'answer':answer 
        })
if __name__=='__main__':
    app.run(port=5000)


