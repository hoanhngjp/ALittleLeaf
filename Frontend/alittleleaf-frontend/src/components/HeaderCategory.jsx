import { Link } from 'react-router-dom'
import { useCategories } from '../hooks/useCategories'

export default function HeaderCategory() {
  const { data: categories = [] } = useCategories()

  return (
    <ul className="dropDown">
      {categories.map((cat) => (
        <li key={cat.categoryId}>
          <Link to={`/collections/${cat.categoryId}`}>
            {cat.categoryName}
            {cat.subCategories?.length > 0 && (
              <i className="fa-solid fa-chevron-right" style={{ fontSize: '11px', marginLeft: '6px' }} />
            )}
          </Link>

          {cat.subCategories?.length > 0 && (
            <ul className="subMenu">
              {cat.subCategories.map((sub) => (
                <li key={sub.categoryId}>
                  <Link to={`/collections/${sub.categoryId}`}>{sub.categoryName}</Link>
                </li>
              ))}
            </ul>
          )}
        </li>
      ))}
    </ul>
  )
}
