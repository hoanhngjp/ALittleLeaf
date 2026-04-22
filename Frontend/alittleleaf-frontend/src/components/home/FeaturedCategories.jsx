import { Link } from 'react-router-dom'
import { useCategories } from '../../hooks/useCategories'

export default function FeaturedCategories() {
  const { data: categories = [], isLoading } = useCategories()

  const topLevel = categories.filter((c) => !c.categoryParentId)

  if (isLoading) return null

  return (
    <>
      <div className="category-title">
        <h2>DANH MỤC SẢN PHẨM</h2>
      </div>
      <div className="category-wrap">
        <div className="row g-3">
          {topLevel.map((cat) => (
            <div className="col-12 col-md-3 category-item" key={cat.categoryId}>
              <Link to={`/collections/${cat.categoryId}`}>
                <div className="category-item-img">
                  <img src={cat.categoryImg} alt={cat.categoryName} />
                </div>
                <span>{cat.categoryName}</span>
              </Link>
            </div>
          ))}
        </div>
      </div>
    </>
  )
}
