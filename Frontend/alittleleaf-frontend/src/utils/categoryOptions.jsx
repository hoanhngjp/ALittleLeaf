/**
 * Renders a flat, visually-indented list of <option> elements from a
 * nested CategoryDto tree (parent → subCategories[]).
 * Every item — parent and child — is selectable.
 *
 * Usage:
 *   <select>
 *     <option value="">— placeholder —</option>
 *     <CategoryOptions categories={data} />
 *   </select>
 */
export function CategoryOptions({ categories }) {
  const catArray = Array.isArray(categories) ? categories : (categories?.data ?? [])
  const roots    = catArray.filter((c) => !c.categoryParentId)

  const getChildren = (parent) =>
    parent.subCategories?.length > 0
      ? parent.subCategories
      : catArray.filter((c) => c.categoryParentId === parent.categoryId)

  return roots.flatMap((parent) => {
    const children = getChildren(parent)
    return [
      <option key={parent.categoryId} value={parent.categoryId}>
        {parent.categoryName}
      </option>,
      ...children.map((child) => (
        <option key={child.categoryId} value={child.categoryId}>
          {'\u00A0\u00A0\u00A0\u00A0'}—{'\u00A0'}{child.categoryName}
        </option>
      )),
    ]
  })
}
