import { useEffect, useState } from 'react'
import './App.css'

function App() {
  //hook
  const [count, setCount] = useState(0)
  const [data, setData] = useState<any[]>([])

  //ajax?
  useEffect(() => {
    loadData();
  }, []);

  async function loadData() {
    const resp = await fetch("https://localhost:7082/api/Books");
    const data = await resp.json();
    setData(data)
  }

  //javascript -> js -> jsx
  //typescript -> ts -> tsx
  //map -> Select c#
  return (
    <>
      <div className="card">
        <button onClick={() => setCount((count) => count + 1)}>
          count is!!! {count}
        </button>

        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Title</th>
              <th>Author</th>
              <th>Publication Year</th>
              <th>Category</th>
            </tr>
          </thead>
          <tbody>
            {data.map(b => <tr>
              <td>{b.id}</td>
              <td>{b.title}</td>
              <td>{b.author}</td>
              <td>{b.publicationYear}</td>
              <td>{b.categoryName}</td>
            </tr>)}
          </tbody>
        </table>

      </div>
    </>
  )
}

export default App
